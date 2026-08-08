document.addEventListener('DOMContentLoaded', function () {
    const form = document.getElementById('room-form');

    if (!form) {
        return;
    }

    const translated = (key, fallback) => form.dataset[key] || fallback;
    const mode = form.dataset.mode || 'create';
    const roomId = Number.parseInt(form.dataset.roomId || '0', 10);
    const fields = {
        name: document.getElementById('name'),
        capacity: document.getElementById('capacity-input'),
        location: document.getElementById('location-select'),
        gear: document.getElementById('gear-select')
    };
    const editButton = document.getElementById('edit-form');
    const saveButton = document.getElementById('save-form');
    const cancelButton = document.getElementById('cancel-edit');
    const submitButton = saveButton || form.querySelector('button[type="submit"]');
    const formWorkspace = form.closest('[data-form-workspace]');
    const statusText = document.getElementById('form-status-text');
    const originalSubmitContent = submitButton?.innerHTML;
    const touchedFields = new Set();
    const choicesInstances = [];

    const createChoices = (element, options) => {
        if (!element || typeof Choices === 'undefined') {
            return null;
        }

        const instance = new Choices(element, options);
        choicesInstances.push(instance);

        if (element.disabled) {
            instance.disable();
        }

        return instance;
    };

    const gearPlaceholder = translated('selectEquipment', 'Vyberte vybavení...');
    const gearChoices = createChoices(fields.gear, {
        removeItemButton: true,
        searchEnabled: true,
        searchPlaceholderValue: translated('searchEquipment', 'Hledat vybavení...'),
        placeholderValue: gearPlaceholder,
        noChoicesText: translated('noEquipment', 'Žádné další vybavení k výběru'),
        itemSelectText: translated('clickToSelect', 'Kliknutím vyberte')
    });

    const syncGearPlaceholder = () => {
        if (!gearChoices) {
            return;
        }

        const hasSelectedGear = Array.from(fields.gear.selectedOptions)
            .some(option => option.value !== '');

        gearChoices.input.placeholder = hasSelectedGear ? '' : gearPlaceholder;
        gearChoices.input.setWidth();
    };

    fields.gear?.addEventListener('addItem', syncGearPlaceholder);
    fields.gear?.addEventListener('removeItem', syncGearPlaceholder);
    syncGearPlaceholder();

    createChoices(fields.location, {
        removeItemButton: false,
        searchEnabled: true,
        searchPlaceholderValue: translated('searchLocations', 'Hledat umístění...'),
        placeholderValue: translated('selectLocation', 'Vyberte umístění...'),
        noChoicesText: translated('noLocations', 'Žádné další umístění k výběru'),
        itemSelectText: translated('clickToSelect', 'Kliknutím vyberte')
    });

    const showToast = (message, background = '#dc3545') => {
        Toastify({
            text: message,
            duration: 5000,
            close: true,
            style: { background }
        }).showToast();
    };

    const getValidationTarget = field => {
        if (!field || field.tagName !== 'SELECT') {
            return field;
        }

        return field.closest('.form-group')?.querySelector('.choices') || field;
    };

    const setFieldError = (field, message) => {
        if (!field) {
            return;
        }

        const target = getValidationTarget(field);
        target.classList.toggle('is-invalid', Boolean(message));
        field.setAttribute('aria-invalid', message ? 'true' : 'false');

        let feedback = target.parentElement.querySelector(`[data-validation-for="${field.id}"]`);
        if (!feedback) {
            feedback = document.createElement('div');
            feedback.className = 'invalid-feedback client-validation-message';
            feedback.dataset.validationFor = field.id;
            target.insertAdjacentElement('afterend', feedback);
        }

        feedback.textContent = message || '';
        feedback.style.display = message ? 'block' : 'none';
    };

    const getErrors = () => {
        const errors = new Map();
        const name = fields.name.value.trim();
        const capacity = Number(fields.capacity.value);

        if (!name) {
            errors.set(fields.name, translated('nameRequired', 'Vyplňte název místnosti.'));
        } else if (name.length < 5 || name.length > 120) {
            errors.set(fields.name, translated('nameLength', 'Název místnosti musí mít 5 až 120 znaků.'));
        }

        if (!Number.isInteger(capacity) || capacity < 1 || capacity > 1000) {
            errors.set(fields.capacity, translated('capacityRange', 'Kapacita musí být od 1 do 1000.'));
        }

        if (!Number(fields.location.value)) {
            errors.set(fields.location, translated('locationRequired', 'Vyberte umístění.'));
        }

        return errors;
    };

    const validateForm = showAllErrors => {
        const errors = getErrors();

        [fields.name, fields.capacity, fields.location].forEach(field => {
            const shouldShow = showAllErrors || touchedFields.has(field.id) || field.getAttribute('aria-invalid') === 'true';
            if (shouldShow) {
                setFieldError(field, errors.get(field) || '');
            }
        });

        return errors;
    };

    [fields.name, fields.capacity, fields.location].forEach(field => {
        field.addEventListener('blur', () => {
            touchedFields.add(field.id);
            validateForm(false);
        });

        field.addEventListener('input', () => {
            if (touchedFields.has(field.id)) {
                validateForm(false);
            }
        });

        field.addEventListener('change', () => {
            touchedFields.add(field.id);
            validateForm(false);
        });
    });

    const setEditing = enabled => {
        form.querySelectorAll('input, select, textarea').forEach(control => {
            control.disabled = !enabled;
        });

        choicesInstances.forEach(instance => enabled ? instance.enable() : instance.disable());
        formWorkspace?.classList.toggle('is-readonly', !enabled);
        formWorkspace?.classList.toggle('is-editing', enabled);
        if (statusText) {
            statusText.textContent = translated(enabled ? 'editStatus' : 'viewStatus', enabled ? 'Edit mode' : 'View mode');
        }
        editButton.hidden = enabled;
        saveButton.hidden = !enabled;
        cancelButton.hidden = !enabled;

        if (enabled) {
            fields.name.focus();
        }
    };

    const setSubmitting = enabled => {
        submitButton.disabled = enabled;
        submitButton.setAttribute('aria-busy', enabled ? 'true' : 'false');
        submitButton.textContent = enabled
            ? translated('savingLabel', 'Saving...')
            : '';
        if (!enabled) {
            submitButton.innerHTML = originalSubmitContent;
        }
    };

    if (mode === 'edit') {
        editButton.addEventListener('click', () => setEditing(true));
        cancelButton.addEventListener('click', () => window.location.reload());
    }

    form.addEventListener('submit', async function (event) {
        event.preventDefault();
        [fields.name, fields.capacity, fields.location].forEach(field => touchedFields.add(field.id));
        const errors = validateForm(true);

        if (errors.size > 0) {
            showToast(translated('validationSummary', 'Opravte zvýrazněná pole.'));
            getValidationTarget(errors.keys().next().value)?.focus();
            return;
        }

        const data = {
            Name: fields.name.value.trim(),
            Capacity: Number.parseInt(fields.capacity.value, 10),
            LocationId: Number.parseInt(fields.location.value, 10),
            GearIds: Array.from(fields.gear.selectedOptions)
                .map(option => Number.parseInt(option.value, 10))
                .filter(Number.isInteger)
        };

        setSubmitting(true);

        try {
            const response = await fetch(mode === 'edit' ? `/rooms/${roomId}` : '/rooms', {
                method: mode === 'edit' ? 'PUT' : 'POST',
                headers: { 'Content-Type': 'application/json' },
                body: JSON.stringify(data)
            });

            if (!response.ok) {
                let problem = null;
                try {
                    problem = await response.json();
                } catch (_) {
                    // Odpověď nemusí obsahovat JSON.
                }

                if (problem?.errors) {
                    Object.values(problem.errors).flat().forEach(showToast);
                } else {
                    showToast(problem?.detail || problem?.title || translated('serverError', 'Chyba serveru při ukládání.'));
                }
                return;
            }

            await Swal.fire({
                title: translated('successTitle', 'Dobrá práce!'),
                text: translated('successMessage', 'Místnost byla úspěšně uložena!'),
                icon: 'success'
            });
            window.location.href = mode === 'edit' ? `/detail/room/${roomId}` : '/dashboard/room';
        } catch (error) {
            showToast(translated('connectionError', 'Došlo k chybě připojení k serveru.'));
        } finally {
            setSubmitting(false);
        }
    });
});

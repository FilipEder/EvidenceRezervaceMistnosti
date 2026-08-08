document.addEventListener('DOMContentLoaded', function () {
    const form = document.getElementById('reservation-form');

    if (!form) {
        return;
    }

    const translated = (key, fallback) => form.dataset[key] || fallback;
    const mode = form.dataset.mode || 'create';
    const reservationId = Number.parseInt(form.dataset.reservationId || '0', 10);
    const roomSelect = document.getElementById('room-select') || document.getElementById('RoomId');
    const fields = {
        name: document.getElementById('Name'),
        lastName: document.getElementById('LastName'),
        email: document.getElementById('Email'),
        room: roomSelect,
        people: document.getElementById('NumberOfPeople'),
        day: document.getElementById('Day'),
        timeFrom: document.getElementById('TimeFrom'),
        timeTo: document.getElementById('TimeTo'),
        description: document.getElementById('Description')
    };
    const rooms = Array.from(roomSelect?.options || [])
        .filter(option => option.value && Number.isFinite(Number(option.dataset.capacity)))
        .map((option, originalOrder) => ({
            value: option.value,
            label: option.textContent.trim(),
            capacity: Number(option.dataset.capacity),
            selected: option.selected,
            originalOrder
        }));
    const roomsById = new Map(rooms.map(room => [room.value, room]));
    const editButton = document.getElementById('edit-form');
    const saveButton = document.getElementById('save-form');
    const cancelButton = document.getElementById('cancel-edit');
    const cancelReservationButton = document.getElementById('cancel-reservation');
    const submitButton = saveButton || form.querySelector('button[type="submit"]');
    const formWorkspace = form.closest('[data-form-workspace]');
    const statusText = document.getElementById('form-status-text');
    const descriptionCount = document.getElementById('description-count');
    const roomCapacityHint = document.getElementById('room-capacity-hint');
    const originalSubmitContent = submitButton?.innerHTML;
    const touchedFields = new Set();
    let occupiedIntervals = [];
    let availabilityRequest = 0;
    let roomChoices = null;
    let formEditable = mode !== 'edit';
    let eligibleRooms = [];

    if (roomSelect && typeof Choices !== 'undefined') {
        roomChoices = new Choices(roomSelect, {
            removeItemButton: false,
            searchEnabled: true,
            searchPlaceholderValue: translated('searchRooms', 'Hledat místnosti...'),
            placeholderValue: translated('selectRoom', 'Vyberte místnost...'),
            noChoicesText: translated('noRooms', 'Žádné další místnosti k výběru'),
            itemSelectText: translated('clickToSelect', 'Kliknutím vyberte'),
            shouldSort: false
        });

        if (roomSelect.disabled) {
            roomChoices.disable();
        }
    }

    const showToast = (message, background = '#dc3545') => {
        Toastify({
            text: message,
            duration: 5000,
            close: true,
            style: { background }
        }).showToast();
    };

    const timeToMinutes = value => {
        if (!/^\d{2}:\d{2}$/.test(value || '')) {
            return null;
        }

        const [hours, minutes] = value.split(':').map(Number);
        return (hours * 60) + minutes;
    };

    const isValidDate = value => {
        const match = /^(\d{4})-(\d{2})-(\d{2})$/.exec(value || '');
        if (!match) {
            return false;
        }

        const year = Number(match[1]);
        const month = Number(match[2]);
        const day = Number(match[3]);
        const date = new Date(Date.UTC(year, month - 1, day));

        return date.getUTCFullYear() === year &&
            date.getUTCMonth() === month - 1 &&
            date.getUTCDate() === day;
    };

    const getValidationTarget = field => {
        if (!field || field.tagName !== 'SELECT') {
            return field;
        }

        const fieldGroup = field.closest('.form-group, [class*="col-"]');
        return fieldGroup?.querySelector('.choices') || field;
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
        const lastName = fields.lastName.value.trim();
        const email = fields.email.value.trim();
        const people = Number(fields.people.value);
        const roomCapacity = roomsById.get(fields.room.value)?.capacity || 0;
        const from = timeToMinutes(fields.timeFrom.value);
        const to = timeToMinutes(fields.timeTo.value);
        const description = fields.description.value.trim();

        if (!name) {
            errors.set(fields.name, translated('nameRequired', 'Vyplňte jméno.'));
        } else if (name.length < 2 || name.length > 30) {
            errors.set(fields.name, translated('nameLength', 'Jméno musí mít 2 až 30 znaků.'));
        }

        if (!lastName) {
            errors.set(fields.lastName, translated('lastNameRequired', 'Vyplňte příjmení.'));
        } else if (lastName.length < 2 || lastName.length > 30) {
            errors.set(fields.lastName, translated('lastNameLength', 'Příjmení musí mít 2 až 30 znaků.'));
        }

        if (!email || !fields.email.validity.valid) {
            errors.set(fields.email, translated('emailInvalid', 'Zadejte platnou e-mailovou adresu.'));
        }

        if (!fields.people.value || !Number.isInteger(people) || people < 1 || people > 1000) {
            errors.set(fields.people, translated('peopleRange', 'Počet osob musí být od 1 do 1000.'));
        } else {
            if (!Number(fields.room.value)) {
                errors.set(fields.room, translated('roomRequired', 'Vyberte místnost.'));
            } else if (roomCapacity > 0 && people > roomCapacity) {
                errors.set(fields.people, translated('capacityExceeded', 'Počet osob překračuje kapacitu místnosti.'));
            }
        }

        if (!fields.day.value) {
            errors.set(fields.day, translated('dayRequired', 'Vyberte datum rezervace.'));
        } else if (!isValidDate(fields.day.value)) {
            errors.set(fields.day, translated('dayInvalid', 'Zadejte platné datum.'));
        } else if (fields.day.value < fields.day.min || fields.day.value > fields.day.max) {
            errors.set(fields.day, translated('dayRange', 'Datum musí být nejdříve dnes a nejpozději za rok.'));
        }

        if (from === null) {
            errors.set(fields.timeFrom, translated('timeFromRequired', 'Vyberte začátek rezervace.'));
        }

        if (to === null) {
            errors.set(fields.timeTo, translated('timeToRequired', 'Vyberte konec rezervace.'));
        }

        if (from !== null && to !== null) {
            if (to <= from) {
                errors.set(fields.timeTo, translated('timeOrder', 'Konec musí být po začátku rezervace.'));
            } else if (occupiedIntervals.some(interval => from < interval.to && interval.from < to)) {
                errors.set(fields.timeTo, translated('timeConflict', 'Vybraný čas se překrývá s existující rezervací.'));
            }
        }

        if (description && (description.length < 4 || description.length > 500)) {
            errors.set(fields.description, translated('descriptionLength', 'Popis musí být prázdný nebo mít 4 až 500 znaků.'));
        }

        return errors;
    };

    const validateForm = showAllErrors => {
        const errors = getErrors();

        Object.values(fields).forEach(field => {
            if (!field) {
                return;
            }

            const shouldShow = showAllErrors || touchedFields.has(field.id) || field.getAttribute('aria-invalid') === 'true';
            if (shouldShow) {
                setFieldError(field, errors.get(field) || '');
            }
        });

        return errors;
    };

    const updateTimeOptions = () => {
        const selectedFrom = fields.timeFrom.value;
        const selectedTo = fields.timeTo.value;
        const fromMinutes = timeToMinutes(selectedFrom);

        Array.from(fields.timeFrom.options).forEach(option => {
            if (!option.value) {
                option.disabled = true;
                return;
            }

            const minutes = timeToMinutes(option.value);
            const isOccupied = occupiedIntervals.some(interval => interval.from <= minutes && minutes < interval.to);
            option.disabled = minutes >= (24 * 60) - 30 || isOccupied;
        });

        if (fields.timeFrom.selectedOptions[0]?.disabled) {
            fields.timeFrom.value = '';
        }

        Array.from(fields.timeTo.options).forEach(option => {
            if (!option.value) {
                option.disabled = true;
                return;
            }

            const minutes = timeToMinutes(option.value);
            const hasInvalidOrder = fromMinutes !== null && minutes <= fromMinutes;
            const hasConflict = fromMinutes !== null && occupiedIntervals.some(interval =>
                fromMinutes < interval.to && interval.from < minutes);
            option.disabled = fromMinutes === null || hasInvalidOrder || hasConflict;
        });

        if (selectedTo && !fields.timeTo.selectedOptions[0]?.disabled) {
            fields.timeTo.value = selectedTo;
        } else if (fields.timeTo.selectedOptions[0]?.disabled) {
            fields.timeTo.value = '';
        }

        if (selectedFrom && !fields.timeFrom.selectedOptions[0]?.disabled) {
            fields.timeFrom.value = selectedFrom;
        }
    };

    const loadAvailability = async () => {
        const roomId = Number(fields.room.value);
        const day = fields.day.value;
        const currentRequest = ++availabilityRequest;
        occupiedIntervals = [];

        if (!roomId || !day) {
            updateTimeOptions();
            return;
        }

        const query = new URLSearchParams({ roomId: String(roomId), date: day });
        if (mode === 'edit' && reservationId) {
            query.set('excludeReservationId', String(reservationId));
        }

        try {
            const response = await fetch(`/reservations/availability?${query.toString()}`);
            if (!response.ok) {
                throw new Error(`HTTP ${response.status}`);
            }

            const data = await response.json();
            if (currentRequest !== availabilityRequest) {
                return;
            }

            occupiedIntervals = Array.isArray(data)
                ? data.map(item => ({ from: timeToMinutes(item.timeFrom), to: timeToMinutes(item.timeTo) }))
                    .filter(interval => interval.from !== null && interval.to !== null)
                : [];
            updateTimeOptions();
            validateForm(false);
        } catch (error) {
            if (currentRequest === availabilityRequest) {
                updateTimeOptions();
                showToast(translated('availabilityError', 'Nepodařilo se načíst obsazené termíny.'));
            }
        }
    };

    const setEditing = enabled => {
        formEditable = enabled;

        form.querySelectorAll('input, select, textarea').forEach(control => {
            control.disabled = !enabled;
        });

        formWorkspace?.classList.toggle('is-readonly', !enabled);
        formWorkspace?.classList.toggle('is-editing', enabled);
        if (statusText) {
            statusText.textContent = translated(enabled ? 'editStatus' : 'viewStatus', enabled ? 'Edit mode' : 'View mode');
        }

        editButton.hidden = enabled;
        cancelReservationButton.hidden = enabled;
        saveButton.hidden = !enabled;
        cancelButton.hidden = !enabled;
        syncRoomOptions();

        if (enabled) {
            loadAvailability();
            fields.name.focus();
        }
    };

    const updateDescriptionCount = () => {
        if (descriptionCount) {
            descriptionCount.textContent = String(fields.description?.value.length || 0);
        }
    };

    const getValidPeopleCount = () => {
        const rawValue = fields.people.value.trim();
        const people = Number(rawValue);

        return rawValue && Number.isInteger(people) && people >= 1 && people <= 1000
            ? people
            : null;
    };

    const replaceNativeRoomOptions = (availableRooms, selectedValue, placeholder) => {
        const placeholderOption = new Option(placeholder, '', !selectedValue, !selectedValue);
        placeholderOption.disabled = true;
        roomSelect.replaceChildren(placeholderOption);

        availableRooms.forEach(room => {
            const option = new Option(room.label, room.value, false, room.value === selectedValue);
            option.dataset.capacity = String(room.capacity);
            roomSelect.add(option);
        });
    };

    const setRoomEnabled = enabled => {
        roomSelect.disabled = !enabled;

        if (roomChoices) {
            enabled ? roomChoices.enable() : roomChoices.disable();
        }
    };

    const updateRoomCapacityHint = () => {
        if (!roomCapacityHint) {
            return;
        }

        const people = getValidPeopleCount();
        const selectedRoom = roomsById.get(fields.room.value);

        if (people === null) {
            roomCapacityHint.textContent = translated('firstPeople', 'Nejprve zadejte počet osob.');
        } else if (eligibleRooms.length === 0) {
            roomCapacityHint.textContent = translated('noSuitableRooms', 'Žádná místnost nemá dostatečnou kapacitu.');
        } else if (selectedRoom) {
            roomCapacityHint.textContent = `${translated('capacityLabel', 'Capacity')}: ${selectedRoom.capacity}`;
        } else {
            roomCapacityHint.textContent = translated('suitableRooms', 'Vhodné místnosti: {0}')
                .replace('{0}', String(eligibleRooms.length));
        }
    };

    const syncRoomOptions = () => {
        const people = getValidPeopleCount();
        const previousSelection = fields.room.value;

        eligibleRooms = people === null
            ? []
            : rooms
                .filter(room => room.capacity >= people)
                .sort((first, second) =>
                    (first.capacity - second.capacity) ||
                    (first.originalOrder - second.originalOrder));

        const selectedValue = eligibleRooms.some(room => room.value === previousSelection)
            ? previousSelection
            : '';
        const placeholder = people === null
            ? translated('firstPeople', 'Nejprve zadejte počet osob.')
            : eligibleRooms.length === 0
                ? translated('noSuitableRooms', 'Žádná místnost nemá dostatečnou kapacitu.')
                : translated('selectRoom', 'Vyberte místnost...');

        if (roomChoices) {
            roomChoices.setChoices([
                {
                    value: '',
                    label: placeholder,
                    disabled: true,
                    placeholder: true,
                    selected: !selectedValue
                },
                ...eligibleRooms.map(room => ({
                    value: room.value,
                    label: room.label,
                    selected: room.value === selectedValue,
                    customProperties: { capacity: room.capacity }
                }))
            ], 'value', 'label', true, true, true);

            Array.from(roomSelect.options).forEach(option => {
                const room = roomsById.get(option.value);
                if (room) {
                    option.dataset.capacity = String(room.capacity);
                }
            });
        } else {
            replaceNativeRoomOptions(eligibleRooms, selectedValue, placeholder);
        }

        setRoomEnabled(formEditable && people !== null && eligibleRooms.length > 0);
        updateRoomCapacityHint();

        return previousSelection !== fields.room.value;
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

    const cancelReservation = async () => {
        const confirmation = await Swal.fire({
            title: translated('cancelReservationTitle', 'Zrušit tuto rezervaci?'),
            text: translated('cancelReservationText', 'Rezervace bude zrušena a její termín se uvolní.'),
            icon: 'warning',
            showCancelButton: true,
            confirmButtonText: translated('cancelReservationConfirm', 'Ano, zrušit rezervaci'),
            cancelButtonText: translated('keepReservation', 'Ponechat rezervaci'),
            confirmButtonColor: '#dc3545'
        });

        if (!confirmation.isConfirmed) {
            return;
        }

        cancelReservationButton.disabled = true;

        try {
            const response = await fetch('/reservations/' + reservationId, {
                method: 'DELETE'
            });

            if (!response.ok) {
                let problem = null;
                try {
                    problem = await response.json();
                } catch (_) {
                    // Odpověď nemusí obsahovat JSON.
                }

                showToast(
                    problem?.detail ||
                    problem?.title ||
                    translated('cancelReservationError', 'Chyba serveru při rušení rezervace.')
                );
                return;
            }

            await Swal.fire({
                title: translated('successTitle', 'Dobrá práce!'),
                text: translated('cancelReservationSuccess', 'Rezervace byla úspěšně zrušena!'),
                icon: 'success'
            });
            window.location.href = '/dashboard/reservation';
        } catch (error) {
            showToast(translated('connectionError', 'Došlo k chybě připojení k serveru.'));
        } finally {
            cancelReservationButton.disabled = false;
        }
    };

    Object.values(fields).forEach(field => {
        if (!field) {
            return;
        }

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

    fields.room.addEventListener('change', loadAvailability);
    fields.room.addEventListener('change', updateRoomCapacityHint);
    fields.people.addEventListener('input', () => {
        const selectionChanged = syncRoomOptions();

        if (selectionChanged) {
            loadAvailability();
        }

        validateForm(false);
    });
    fields.day.addEventListener('change', loadAvailability);
    fields.timeFrom.addEventListener('change', updateTimeOptions);
    fields.description?.addEventListener('input', updateDescriptionCount);

    if (mode === 'edit') {
        editButton.addEventListener('click', () => setEditing(true));
        cancelButton.addEventListener('click', () => window.location.reload());
        cancelReservationButton.addEventListener('click', cancelReservation);
    }

    updateTimeOptions();
    updateDescriptionCount();
    syncRoomOptions();
    loadAvailability();

    form.addEventListener('submit', async function (event) {
        event.preventDefault();
        Object.values(fields).forEach(field => field && touchedFields.add(field.id));
        const errors = validateForm(true);

        if (errors.size > 0) {
            showToast(translated('validationSummary', 'Opravte zvýrazněná pole.'));
            const firstInvalidField = errors.keys().next().value;
            const firstInvalidTarget = getValidationTarget(firstInvalidField);

            if (firstInvalidField.disabled) {
                fields.people.focus();
            } else {
                firstInvalidTarget?.focus();
            }
            return;
        }

        const data = {
            Name: fields.name.value.trim(),
            LastName: fields.lastName.value.trim(),
            Email: fields.email.value.trim(),
            RoomId: Number.parseInt(fields.room.value, 10),
            NumberOfPeople: Number.parseInt(fields.people.value, 10),
            DateReservation: fields.day.value,
            TimeFrom: fields.timeFrom.value,
            TimeTo: fields.timeTo.value,
            Description: fields.description.value.trim() || null
        };

        setSubmitting(true);

        try {
            const response = await fetch(mode === 'edit' ? `/reservations/${reservationId}` : '/reservations', {
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
                text: translated('successMessage', 'Rezervace byla úspěšně uložena!'),
                icon: 'success'
            });
            window.location.href = mode === 'edit'
                ? `/detail/reservation/${reservationId}`
                : '/dashboard/reservation';
        } catch (error) {
            showToast(translated('connectionError', 'Došlo k chybě připojení k serveru.'));
        } finally {
            setSubmitting(false);
        }
    });
});

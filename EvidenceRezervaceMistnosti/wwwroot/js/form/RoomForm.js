document.addEventListener('DOMContentLoaded', function () {
    const gearSelect = document.getElementById('gear-select');

    if (gearSelect) {
        const choices = new Choices(gearSelect, {
            removeItemButton: true,
            searchEnabled: true,
            searchPlaceholderValue: 'Hledat vybavení...',
            placeholderValue: 'Vyberte předměty...',
            noChoicesText: 'Žádné další vybavení k výběru',
            itemSelectText: 'Kliknutím vyberte'
        });
    }

    const locationSelect = document.getElementById('location-select');

    if (locationSelect) {
        const choices = new Choices(locationSelect, {
            removeItemButton: false,
            searchEnabled: true,
            searchPlaceholderValue: 'Hledat umístění...',
            placeholderValue: 'Vyberte umístění...',
            noChoicesText: 'Žádné další umístění k výběru',
            itemSelectText: 'Kliknutím vyberte'
        });
    }

    const form = document.querySelector('form');
    form.addEventListener('submit', async function (e) {
        e.preventDefault();

        const formData = new FormData(form);
        const data = {
            Name: formData.get('Name'),
            Capacity: parseInt(formData.get('Capacity')) || 0,
            LocationId: parseInt(formData.get('LocationId')) || 0,
            GearIds: formData.getAll('GearIds').map(id => parseInt(id))
        };

        try {
            const response = await fetch('/rooms', {
                method: 'POST',
                headers: {
                    'Content-Type': 'application/json'
                },
                body: JSON.stringify(data)
            });

            if (response.ok) {
                Swal.fire({
                    title: "Dobrá práce!",
                    text: "Místnost byla úspěšně vytvořena!",
                    icon: "success"
                });

                setTimeout(() => {
                    window.location.href = '/dashboard/room';
                }, 2000);

            } else if (response.status === 400) {
                const problem = await response.json();

                if (problem.errors) {
                    // ASP .NET dictionary
                    for (const field in problem.errors) {
                        problem.errors[field].forEach(errorMsg => {
                            // Zobrazíme červený toast pro každou chybu
                            Toastify({
                                text: errorMsg,
                                duration: 5000,
                                close: true,
                                style: { background: "#dc3545" } // Bootstrap červená
                            }).showToast();
                        });
                    }
                } else {
                    Toastify({
                        text: "Zkontrolujte správnost vyplněných údajů.",
                        style: { background: "#dc3545" }
                    }).showToast();
                }
            } else {
                // Chyba 500 nebo jiný pád serveru
                Toastify({
                    text: "Chyba serveru při ukládání.",
                    style: { background: "#dc3545" }
                }).showToast();
            }
        } catch (error) {
            Toastify({
                text: "Došlo k chybě připojení k serveru.",
                style: { background: "#dc3545" }
            }).showToast();
        }
    });
});
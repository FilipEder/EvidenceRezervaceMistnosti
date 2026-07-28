document.addEventListener('DOMContentLoaded', function () {
    const roomSelect = document.getElementById('location-select');

    if (roomSelect) {
        const choices = new Choices(roomSelect, {
            removeItemButton: false,
            searchEnabled: true,
            searchPlaceholderValue: 'Hledat místnosti...',
            placeholderValue: 'Vyberte místnost...',
            noChoicesText: 'Žádné další místnosti k výběru',
            itemSelectText: 'Kliknutím vyberte'
        });
    }

    const form = document.querySelector('form');
    form.addEventListener('submit', async function (e) {
        e.preventDefault();

        var valCorrect = true;

        const dayInput = document.getElementById('Day')
        if (dayInput.value == "") {
            Toastify({
                text: "Musíš zvolit den rezervace",
                duration: 5000,
                close: true,
                style: { background: "#dc3545" } // Bootstrap červená
            }).showToast();
            valCorrect = false;
        }

        const timeFrom = document.getElementById('TimeFrom')
        if (timeFrom.value == "") {
            Toastify({
                text: "Musíš zvolit čas začátku rezervace",
                duration: 5000,
                close: true,
                style: { background: "#dc3545" } // Bootstrap červená
            }).showToast();
            valCorrect = false;
        }

        const timeTo = document.getElementById('TimeTo')
        if (timeTo.value == "") {
            Toastify({
                text: "Musíš zvolit čas konce rezervace",
                duration: 5000,
                close: true,
                style: { background: "#dc3545" } // Bootstrap červená
            }).showToast();
            valCorrect = false;
        }

        if (!valCorrect) {
            return;
        }

        const formData = new FormData(form);
        console.log('Day')
        console.log(formData.get('Day'))
        const data = {
            Name: formData.get('Name'),
            LastName: formData.get('LastName'),
            Email: formData.get('Email'),
            RoomId: parseInt(formData.get('RoomId')) || 0,
            NumberOfPeople: parseInt(formData.get('NumberOfPeople')) || 0,
            DateReservation:  formData.get('Day') || null,
            TimeFrom: formData.get('TimeFrom') || null,
            TimeTo: formData.get('TimeTo') || null,
            Description: formData.get('Description')
        };

        console.log(data)

        try {
            const response = await fetch('/reservations', {
                method: 'POST',
                headers: {
                    'Content-Type': 'application/json'
                },
                body: JSON.stringify(data)
            });

            if (response.ok) {
                Swal.fire({
                    title: "Dobrá práce!",
                    text: "Rezervace byla úspěšně vytvořena!",
                    icon: "success"
                });

                setTimeout(() => {
                    window.location.href = '/dashboard/reservation';
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
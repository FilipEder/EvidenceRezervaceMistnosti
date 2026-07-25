

window.onload = function () {
    const detailChangeContainer = document.querySelectorAll('.primaryDetailContainer');

    detailChangeContainer.forEach(function (link) {
        link.addEventListener('click', function (event) {
            var el = event.target;
            if (!el.classList.contains('selected')) {
                var selectedElements = document.querySelectorAll('.selected');
                if (selectedElements.length > 0) {
                    selectedElements[0].classList.remove('selected');
                }
                el.classList.add('selected');
            }
        })
    })

    const detailChangeLink = document.querySelectorAll('.primaryDetailLink')

    detailChangeLink.forEach(function (link) {
        link.addEventListener('click', function (event) {
            event.preventDefault();
        })
    })
}
window.onload = function () {
    const detailChangeContainer = document.querySelectorAll('.detail-tab');

    detailChangeContainer.forEach(function (link) {
        link.addEventListener('click', function (event) {
            var el = event.target;
            if (!el.classList.contains('active')) {
                var selectedElements = document.querySelectorAll('.active');
                if (selectedElements.length > 0) {
                    selectedElements[0].classList.remove('active');
                }
                el.classList.add('active');
            }
        })
    })
}

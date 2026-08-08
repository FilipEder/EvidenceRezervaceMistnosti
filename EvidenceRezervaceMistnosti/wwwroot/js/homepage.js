document.addEventListener('DOMContentLoaded', initDashboard);

function initDashboard() {
    initDashboardTabs();
    initDashboardSearchSelects();
}

function initDashboardTabs() {
    const detailTabs = document.querySelectorAll('.detail-tab');

    detailTabs.forEach(tab => {
        tab.addEventListener('click', event => {
            const activeTab = document.querySelector('.detail-tab.active');

            if (activeTab) {
                activeTab.classList.remove('active');
            }

            event.currentTarget.classList.add('active');
        });
    });
}

function initDashboardSearchSelects() {
    if (typeof Choices === 'undefined') {
        return;
    }

    const searchSelects = document.querySelectorAll('[data-choices-search]');

    searchSelects.forEach(select => {
        const choices = new Choices(select, {
            allowHTML: false,
            searchEnabled: true,
            searchFields: ['label'],
            searchPlaceholderValue: select.dataset.choicesSearch,
            noResultsText: select.dataset.choicesNoResults,
            noChoicesText: select.dataset.choicesNoOptions,
            itemSelectText: select.dataset.choicesSelectText,
            shouldSort: false
        });

        if (select.disabled) {
            choices.disable();
        }
    });
}

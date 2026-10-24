// Light / dark theme toggle (the initial theme is set by an inline script in _Layout.cshtml).
(function () {
    var button = document.getElementById('themeToggle');
    if (!button) return;
    button.addEventListener('click', function () {
        var root = document.documentElement;
        var next = root.getAttribute('data-bs-theme') === 'dark' ? 'light' : 'dark';
        root.setAttribute('data-bs-theme', next);
        try { localStorage.setItem('mynotes-theme', next); } catch (e) { }
    });
})();

// Fade out "note saved / deleted" messages after a few seconds.
(function () {
    document.querySelectorAll('.js-auto-dismiss').forEach(function (el) {
        setTimeout(function () {
            if (window.bootstrap && bootstrap.Alert) {
                bootstrap.Alert.getOrCreateInstance(el).close();
            } else {
                el.remove();
            }
        }, 3500);
    });
})();

// Ctrl+Enter (Cmd+Enter on Mac) submits the form you're typing in.
document.addEventListener('keydown', function (e) {
    if ((e.ctrlKey || e.metaKey) && e.key === 'Enter') {
        var form = e.target.closest && e.target.closest('form.js-ctrl-enter');
        if (form) {
            e.preventDefault();
            form.requestSubmit ? form.requestSubmit() : form.submit();
        }
    }
});

// Theme management
document.addEventListener('DOMContentLoaded', function () {
    const storageKey = 'theme';
    const root = document.documentElement;
    const saved = localStorage.getItem(storageKey) || 'light';
    
    // Apply saved theme
    root.setAttribute('data-theme', saved);
    
    // Wait a bit for buttons to be rendered
    setTimeout(function() {
        updateThemeButtons(saved);
        
        // Theme toggle buttons
        const themeButtons = document.querySelectorAll('.theme-btn');
        themeButtons.forEach(btn => {
            btn.addEventListener('click', function () {
                const theme = this.getAttribute('data-theme');
                root.setAttribute('data-theme', theme);
                localStorage.setItem(storageKey, theme);
                updateThemeButtons(theme);
            });
        });
    }, 100);
    
    function updateThemeButtons(activeTheme) {
        const themeButtons = document.querySelectorAll('.theme-btn');
        themeButtons.forEach(btn => {
            const btnTheme = btn.getAttribute('data-theme');
            if (btnTheme === activeTheme) {
                btn.classList.add('active');
            } else {
                btn.classList.remove('active');
            }
        });
    }
});

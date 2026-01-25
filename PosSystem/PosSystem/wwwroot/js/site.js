// Enable transitions after Blazor has loaded
Blazor.start().then(() => {
    document.documentElement.classList.add('transitions-enabled');
});
let mermaidPromise = null;

function loadMermaid() {
    mermaidPromise ??= import('https://cdn.jsdelivr.net/npm/mermaid@11/dist/mermaid.esm.min.mjs')
        .then(module => {
            module.default.initialize({ startOnLoad: false, theme: 'default' });
            return module.default;
        });
    return mermaidPromise;
}

window.renderMermaidDiagram = async function (elementId, definition) {
    const container = document.getElementById(elementId);
    if (!container) {
        return;
    }

    const mermaid = await loadMermaid();
    const renderId = `${elementId}-svg-${Date.now()}`;
    const { svg } = await mermaid.render(renderId, definition);
    container.innerHTML = svg;
};

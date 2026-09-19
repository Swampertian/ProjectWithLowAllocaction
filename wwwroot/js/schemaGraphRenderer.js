let cytoscapePromise = null;

function loadCytoscape() {
    cytoscapePromise ??= import('https://cdn.jsdelivr.net/npm/cytoscape@3/+esm')
        .then(module => module.default);
    return cytoscapePromise;
}

const instances = new Map();
const INITIAL_ZOOM = 0.3;

function themeColor(name, fallback) {
    const value = getComputedStyle(document.documentElement).getPropertyValue(name).trim();
    return value || fallback;
}

function tableLabel(table) {
    const divider = '─'.repeat(Math.min(Math.max(table.name.length, 8), 24));
    const columns = table.columns.map(c => `${c.name}: ${c.type}`);
    return [table.name, divider, ...columns].join('\n');
}

function buildElements(graph) {
    return graph.tables.map((table, index) => ({
        data: {
            id: `table-${index}`,
            label: tableLabel(table)
        }
    }));
}

async function createInstance(elementId, graph) {
    const cytoscape = await loadCytoscape();
    const container = document.getElementById(elementId);
    if (!container) {
        return null;
    }

    const cy = cytoscape({
        container,
        elements: buildElements(graph),
        style: [
            {
                selector: 'node',
                style: {
                    shape: 'round-rectangle',
                    label: 'data(label)',
                    'text-wrap': 'wrap',
                    'text-valign': 'center',
                    'text-halign': 'center',
                    'text-justification': 'left',
                    'font-family': "'SF Mono', 'Fira Code', monospace",
                    'font-size': 12,
                    padding: '14px',
                    width: 'label',
                    height: 'label',
                    'background-color': themeColor('--bg-surface', '#1E1E1E'),
                    'border-width': 1,
                    'border-color': themeColor('--border', '#333333'),
                    color: themeColor('--text-primary', '#E0E0E0')
                }
            }
        ],
        layout: { name: 'grid', avoidOverlap: true, avoidOverlapPadding: 40, fit: false },
        wheelSensitivity: 0.2,
        minZoom: 0.2,
        maxZoom: 3
    });

    cy.zoom(INITIAL_ZOOM);
    cy.center();

    instances.set(elementId, { cy, tableNames: graph.tables.map(t => t.name) });
    return cy;
}

function updateInstance(entry, graph) {
    const { cy } = entry;
    const nextNames = graph.tables.map(t => t.name);
    const sameShape = nextNames.length === entry.tableNames.length
        && nextNames.every((name, i) => name === entry.tableNames[i]);

    const elements = buildElements(graph);
    if (sameShape) {
        elements.forEach(el => {
            const node = cy.getElementById(el.data.id);
            if (node.nonempty()) {
                node.data('label', el.data.label);
            }
        });
    } else {
        cy.elements().remove();
        cy.add(elements);
        cy.layout({ name: 'grid', avoidOverlap: true, avoidOverlapPadding: 40, fit: false }).run();
        cy.zoom(INITIAL_ZOOM);
        cy.center();
        entry.tableNames = nextNames;
    }
}

window.renderSchemaGraph = async function (elementId, graph) {
    const existing = instances.get(elementId);
    if (existing) {
        updateInstance(existing, graph);
        return;
    }

    await createInstance(elementId, graph);
};

window.disposeSchemaGraph = function (elementId) {
    const existing = instances.get(elementId);
    if (existing) {
        existing.cy.destroy();
        instances.delete(elementId);
    }
};

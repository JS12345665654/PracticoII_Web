document.addEventListener('DOMContentLoaded', function () {
    let urlEliminar = null;
    const modalEliminar = new bootstrap.Modal(document.getElementById('confirmEliminarModal'));
    const btnConfirmarEliminar = document.getElementById('btnConfirmarEliminar');

    // Al hacer clic en cualquier botón eliminar
    document.querySelectorAll('.btn-eliminar').forEach(button => {
        button.addEventListener('click', function () {
            urlEliminar = this.getAttribute('data-url');
            modalEliminar.show();
        });
    });

    // Al confirmar eliminación
    btnConfirmarEliminar.addEventListener('click', function () {
        if (!urlEliminar) return;

        // Crear formulario dinámico para enviar POST
        const form = document.createElement('form');
        form.method = 'post';
        form.action = urlEliminar;

        document.body.appendChild(form);
        form.submit();

        modalEliminar.hide();
    });
});

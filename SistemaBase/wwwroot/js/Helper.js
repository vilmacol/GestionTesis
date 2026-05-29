

const bloquearFormulario = (datosFormulario) => {

    var elements = datosFormulario.elements;
    console.log({ 'elements1 ': elements });

    for (var i = 0; i < elements.length; i++) {
        var element = elements[i];
        console.log({ 'tagName': element.tagName })
        if (element.tagName === 'SELECT') {
            deshabilitarSelect(element);
            reinicializarChoices(element);
        } else {
            element.disabled = true;
        }
    }
}

const desbloquearFormulario = (datosFormulario) => {
    var elements = datosFormulario.elements;
    console.log({ 'elements2 ': elements });
    for (var i = 0; i < elements.length; i++) {
        var element = elements[i];
        if (element.tagName === 'SELECT') {
            element.readOnly = false;
        } else {
            element.disabled = false;
        }
    }
}

const deshabilitarSelect = (selectElement) => {
    selectElement.disabled = true;
    selectElement.setAttribute('readonly', 'readonly');
    selectElement.classList.add('disabled');
    selectElement.addEventListener('click', desactivarEventosSelect);
}
const desactivarEventosSelect = (event) => {
    event.preventDefault();
    event.stopPropagation();
}


const reinicializarChoices = (selectElement) => {
    var choices = new Choices(selectElement, {
        removeItemButton: true,
        placeholder: true
    });
}

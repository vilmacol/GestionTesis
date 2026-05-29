const validarPassword = () => {
    const pass = document.getElementById("pass").value;
    const confirmarPass = document.getElementById("confirmarPass").value;
    const errorPass = document.getElementById("error-pass");
    const errorConfirmar = document.getElementById("error-confirmar");
    const btnGuardar = document.getElementById("btnGuardar");

    // Expresión regular: 
    // ^(?=.*[A-Z])  -> Al menos una mayúscula
    // (?=.*[\W_])   -> Al menos un carácter especial
    // .{1,8}$       -> minimo 8
    const regex = /^(?=.*[A-Z])(?=.*[\W_]).{8,}$/;

    let valido = true;
    if (pass.length < 8) {
        errorPass.innerText = "La contraseña debe tener al menos 8 caracteres.";
        valido = false;
    } else if (!regex.test(pass)) {
        errorPass.innerText = "La contraseña debe incluir al menos una mayúscula y un carácter especial.";
        valido = false;
    } else {
        errorPass.innerText = "";
         valido = true;
    }


    if (pass !== confirmarPass) {
        errorConfirmar.innerText = "Las contraseñas no coinciden.";
        valido = false;
    } else {
        errorConfirmar.innerText = "";
         valido = true;
    }

    // Habilita o deshabilita el botón de guardar
    btnGuardar.disabled = !valido;

};


const actualizarPassword = async (event) => {
    event.preventDefault();

    const loader = document.getElementById("loader_login");
    loader.style.visibility = "visible";

    const formData = new FormData();
    formData.append("usuario", document.getElementById("usuario").value);
    formData.append("pass", document.getElementById("pass").value);
    formData.append("confirmarPass", document.getElementById("confirmarPass").value);

    try {
        // Paso 1: Actualizar la contraseña (PrimerLogin)
        const response1 = await axios.post("/Login/ActualizarPassword", formData, {
            headers: { "Content-Type": "multipart/form-data", 'X-Response-View': 'Json' }
        });

        if (!response1.data.success) {
            alert(response1.data.message);
            return;
        }

        // Paso 2: Si se actualizó la contraseña, procede con el login
        const response2 = await axios.post("/Login/Login", formData, {
            headers: { "Content-Type": "multipart/form-data", 'X-Response-View': 'Json' }
        });

        if (response2.data.success) {
            window.location.href = response2.data.redirect;
        } else {
            alert(response2.data.message);
        }

    } catch (error) {
        console.error("Error en la solicitud:", error);
        alert('Error al procesar la solicitud. Intente nuevamente.');
    } finally {
        loader.style.visibility = "hidden";
    }
};




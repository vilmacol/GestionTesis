const ingresar = async (event) => {
    event.preventDefault();
    document.getElementById("loader_login").style.visibility = "visible";

    const formLogin = document.getElementById('FormLogin');
    const formData = new FormData(formLogin);

    try {
        let response = await axios.post("Login/PrimerLogin", formData, {
            headers: { "Content-Type": "multipart/form-data", 'X-Response-View': 'Json' }
        });

      ////  if (!response.data.success) {
            if (response.data.redirect) {
                window.location.href = response.data.redirect;
                return;
            }
            // Swal.fire({
            //     icon: 'error',
            //     title: 'Error',
            //     text: response.data.message,
            // });
            // return;
            // alert(response.data.message);
            // return;
       // }

        // Si pasa PrimerLogin, procede con Login
        response = await axios.post("Login/Login", formData, {
            headers: { "Content-Type": "multipart/form-data", 'X-Response-View': 'Json' }
        });

        if (response.data.success) {
            window.location.href = response.data.redirect;
        } else {
            Swal.fire({
                icon: 'error',
                title: 'Error',
                text: response.data.message,
            });
        }
    } catch (error) {
        Swal.fire({
            icon: 'error',
            title: 'Error',
            text: 'Error al procesar la solicitud',
        });
        //alert('Error al procesar la solicitud');
    } finally {
        document.getElementById("loader_login").style.visibility = "hidden";
    }
};
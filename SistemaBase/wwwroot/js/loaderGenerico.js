const showLoader =  (containerId, width, height) => {
    var container = document.getElementById(containerId);
    container.style.display = 'block';

    var options = {
        container: container,
        renderer: 'svg',
        loop: true,
        autoplay: true,
        path: '/js/loader_lottie.json',
        width: width,
        height: height
    };

    var anim = lottie.loadAnimation(options);
}
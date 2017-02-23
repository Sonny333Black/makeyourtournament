window.addEventListener("load", function () {
    var ws;

    try {
        ws = new WebSocket("ws://localhost:22888/Handler1.ashx");

        ws.onerror = function () {
            console.log('Verbindung fehlgeschlagen');
        };

        ws.onopen = function () {
            console.log("verbunden, readyState: " + this.readyState);
        };

        ws.onmessage = function (e) {
           console.log(e);
           var text = e.data;
           if(text != ""){
               toastr["info"](text, "News")

               toastr.options = {
                   "closeButton": false,
                   "debug": false,
                   "newestOnTop": true,
                   "progressBar": true,
                   "positionClass": "toast-top-center",
                   "preventDuplicates": true,
                   "onclick": null,
                   "showDuration": "500",
                   "hideDuration": "1000",
                   "timeOut": "5000",
                   "extendedTimeOut": "1000",
                   "showEasing": "swing",
                   "hideEasing": "linear",
                   "showMethod": "fadeIn",
                   "hideMethod": "fadeOut"
               }
           }      
           
        };

        ws.onclose = function () {
            console.log("Verbindung getrennt, readyState: " + this.readyState);
        };
    }
    catch (e) {

    }
    
});
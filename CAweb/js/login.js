const baseUri = 'https://localhost:5001/';
const loginUri = baseUri + 'auth/login';

/* document ready function */
$(document).ready(function(){
    $("#log-in-button").click(function(){
        var username = $("#user-id").val().trim();
        var password = $("#user-password").val().trim();

        if( username != "" && password != "" ){
            try {
                login(loginUri, {UserId:username, Password:password});
            }
            catch(error) {
                console.log(error);
            }
        }
    });
});


/* Logout functions */
function login(url = '', data = {}) {
    // Default options are marked with *
    fetch(url, {
        method: 'POST', // *GET, POST, PUT, DELETE, etc.
        mode: 'cors', // no-cors, *cors, same-origin
        cache: 'no-cache', // *default, no-cache, reload, force-cache, only-if-cached
        credentials: 'include', // include, *same-origin, omit (browser does not include credentials in the query)
        headers: {
            'Content-Type': 'application/json',
        },
        redirect: 'follow', // manual, *follow, error
        referrerPolicy: 'no-referrer', // no-referrer, *no-referrer-when-downgrade, origin, origin-when-cross-origin, same-origin, strict-origin, strict-origin-when-cross-origin, unsafe-url
        body: JSON.stringify(data) // body data type must match "Content-Type" header
        }).then(response => response.json())
        .then(jsonData => _validateLoginAndRoute(jsonData))
        .catch(error => console.error('Unable to login', error));
  }

  function _validateLoginAndRoute(response) {
    if (response.isLogin == true) {
        console.log(response.message);
        window.location = "/user"; 
    } 
    else {
        console.log(response.message);
    }
  };
  

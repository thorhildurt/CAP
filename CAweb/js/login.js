const baseUri = 'https://localhost:5001/';
const loginUri = baseUri + 'auth/login';

/* document ready function */
$(document).ready(function(){
    var errorMessageDiv = document.getElementById('login-error-alert');
    errorMessageDiv.classList.remove("show");

    $("#log-in-button").click(function(){
        var username = $("#user-id").val().trim();
        var password = $("#user-password").val().trim();

            if( username != "" && password != "" ) {
                login(loginUri, {UserId:username, Password:password});
            }
            else {
                var messageDiv = document.getElementById('login-error-message');
                messageDiv.innerHTML = `Please enter your login information`;
            
                var errorMessageDiv = document.getElementById('login-error-alert');
                errorMessageDiv.classList.add("show");
    
                console.error('No login information provided', error);
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
        .catch(error => _errorPopup(error));
  }

  function _errorPopup(error) {

    var messageDiv = document.getElementById('login-error-message');
    messageDiv.innerHTML = `A problem has occurred during login`;

    var errorMessageDiv = document.getElementById('login-error-alert');
    errorMessageDiv.classList.add("show");

    console.error('API connection problem during login', error);
  }

  function _validateLoginAndRoute(response) {
    if (response.isLogin == true) {
        console.log(response.message);
        window.location = "/user.html"; 
    } 
    else {
        var messageDiv = document.getElementById('login-error-message');
        messageDiv.innerHTML = `${response.message}`;
    
        var errorMessageDiv = document.getElementById('login-error-alert');
        errorMessageDiv.classList.add("show");

        console.log(response.message);
    }
  };

  

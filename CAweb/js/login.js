const baseUri = 'https://localhost:5001/';
const loginUri = baseUri + 'login';

$(document).ready(function(){
    $("#log-in-button").click(function(){
        var username = $("#user-id").val().trim();
        var password = $("#user-password").val().trim();

        if( username != "" && password != "" ){
            try {
                postData(loginUri, {UserId:username, Password:password})
            }
            catch(error) {
                console.log(error);
            }
        }
    });
});

// Example POST method implementation:
function postData(url = '', input = {}) {
    // Default options are marked with *
    console.log(input);

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
        body: JSON.stringify(input) // body data type must match "Content-Type" header
        }).then(response => response.json())
        .then(data => _validateLoginAndRoute(data, input))
        .catch(error => console.error('Unable to login', error));
  }

  function _validateLoginAndRoute(response, data) {
    if (response.status == true) {
        console.log("Successful login");

        window.location = "/"; 
    } 
    else {
        console.log("Invalid username or password!");
    }
  };
  
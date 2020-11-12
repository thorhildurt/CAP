
const baseUri = 'https://localhost:5001/';
const getUserUri = baseUri + 'user/';
const logoutUri = baseUri + 'auth/logout';

/* get dom objects */
const userDiv = document.getElementById('users');
const userNameHeader = document.getElementById('user-name');
const userEmailHeader = document.getElementById('user-email');

/* document ready function */
$(document).ready(function(){
  $("a#log-out-button").click(function(){
      try {
          logout(logoutUri);
      }
      catch(error) {
          console.log(error);
      }
  });
});

/* User functions */
function getUser() {
    fetch(getUserUri, {
      method: 'GET',
      credentials: 'include'
    }).then(response => response.json())
    .then(data => _displayUserInformation(data))
    .catch(error => _redirect(error)); 
}

// TODO: fix this, this is not smooth...
function _redirect(error) {
  console.log(error);
  window.location = "/login"; 
}

function _displayUserInformation(user) {
    let userNameDiv = createNode('div');
    userNameDiv.innerHTML = `${user.firstName} ${user.lastName}`;
    append(userNameHeader, userNameDiv);

    let emailDiv = createNode('div');
    emailDiv.innerHTML = `Email: ${user.email}`;
    append(userEmailHeader, emailDiv);
}

function createNode(element) {
  return document.createElement(element);
}

function append(parent, element) {
  return parent.appendChild(element);
}

$(document).ready(function(){
  $("#submit-user-info").click(function(event){
      event.preventDefault();
      var body = {
        'FirstName': $("#change-first-name").val().trim(),
        'LastName': $("#change-last-name").val().trim()
      }

      fetch(getUserUri, {
        method: 'PUT', // *GET, POST, PUT, DELETE, etc.
        mode: 'cors', // no-cors, *cors, same-origin
        cache: 'no-cache', // *default, no-cache, reload, force-cache, only-if-cached
        credentials: 'include', // include, *same-origin, omit (browser does not include credentials in the query)
        headers: {
            'Content-Type': 'application/json',
        },
        redirect: 'follow', // manual, *follow, error
        referrerPolicy: 'no-referrer', // no-referrer, *no-referrer-when-downgrade, origin, origin-when-cross-origin, same-origin, strict-origin, strict-origin-when-cross-origin, unsafe-url
        body: JSON.stringify(body) // body data type must match "Content-Type" header
      })
      .then(response => {
        response.json();
        location.reload();
        scrollTo(0, 0);
      })
      .catch(error => console.error('Unable to change user info', error));
  });
});

$(document).ready(function(){
  $("#submit-password-change").click(function(event){
      event.preventDefault();
      var body = {
        'Password': $("#change-password").val().trim(),
        'NewPassword': $("#change-new-password").val().trim()
      };

      fetch(getUserUri, {
        method: 'PUT', // *GET, POST, PUT, DELETE, etc.
        mode: 'cors', // no-cors, *cors, same-origin
        cache: 'no-cache', // *default, no-cache, reload, force-cache, only-if-cached
        credentials: 'include', // include, *same-origin, omit (browser does not include credentials in the query)
        headers: {
            'Content-Type': 'application/json',
        },
        redirect: 'follow', // manual, *follow, error
        referrerPolicy: 'no-referrer', // no-referrer, *no-referrer-when-downgrade, origin, origin-when-cross-origin, same-origin, strict-origin, strict-origin-when-cross-origin, unsafe-url
        body: JSON.stringify(body) // body data type must match "Content-Type" header
      })
      .then(response => {
        response.json();
        location.reload();
        scrollTo(0, 0);
      })
      .catch(error => console.error('Unable to change password', error));
  });
});

/* Logout functions */

function logout(url = '')
{
    fetch(url, {
        method:'POST'
      }).then(response => _logout(response))
      .catch(error => console.error('Unable to logout', error));
}

function _logout(response)
{
  console.log(response);
  window.location = "/login";
}

/* onload activity */
window.onload = getUser;


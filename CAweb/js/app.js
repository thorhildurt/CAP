
const baseUri = 'https://localhost:5001/';
const userUri = baseUri + 'user/';
const logoutUri = baseUri + 'auth/logout';
const certUri = userUri + 'certificates/'; 

/* User functions */
function getUser() {
    fetch(userUri, {
      method: 'GET',
      credentials: 'include'
    }).then(response => response.json())
    .then(data => _displayUserInformation(data))
    .catch(error => _redirect(error)); 
}

// TODO: fix this, this is not smooth...
function _redirect(error) {
  console.log(error);
  window.location = "/login.html"; 
}

function _displayUserInformation(user) {
    $('#user-name')
      .empty()
      .append(`${user.firstName} ${user.lastName}`);

    $('#user-email')
      .empty()
      .append(`Email: ${user.email}`);
}

function getUserCertificates(onlyRevoked=false) {
  fetch(certUri, {
    method: 'GET', // *GET, POST, PUT, DELETE, etc.
    mode: 'cors', // no-cors, *cors, same-origin
    cache: 'no-cache', // *default, no-cache, reload, force-cache, only-if-cached
    credentials: 'include', // include, *same-origin, omit (browser does not include credentials in the query)
    headers: {
      'Content-Type': 'application/json',
    },
    redirect: 'follow', // manual, *follow, error
    referrerPolicy: 'no-referrer', // no-referrer, *no-referrer-when-downgrade, origin, origin-when-cross-origin, same-origin, strict-origin, strict-origin-when-cross-origin, unsafe-url
  })
  .then(response => response.json())
  .then(data => _displayUserCertificates(data, onlyRevoked))
  .catch(error => console.error('Unable to load certificates', error));
}

function _displayError(message) {
  $("#error-alert")
    .empty()
    .append(message)
    .fadeTo(2000, 500)
    .delay(3000)
    .fadeOut(500);
}

function _displaySuccess(message) {
  $("#success-alert")
    .empty()
    .append(message)
    .fadeTo(2000, 500)
    .delay(3000)
    .fadeOut(500);
}

function _displayUserCertificates(data, onlyRevoked) {
  if(!onlyRevoked) {
    $("#certificate-list").empty();
  } else {
    $("#revoked-certificate-list").empty();
  }

  if(data.length != 0) {
    data.forEach(cert => {
      var certEl = document.createElement('li');
      certEl.append(cert['certId']);
      
      if(!onlyRevoked && cert['revoked'] == false) {

        certEl.classList.add('list-group-item', 'list-group-item-success');
        certEl.setAttribute('data-cid', cert['certId']);
        var revokeBtn = document.createElement('button');
        revokeBtn.append('Revoke');
        revokeBtn.classList.add('btn', 'btn-danger', 'btn-sm-custom', 'btn-revoke-certificate');
        certEl.append(revokeBtn);

        $("#certificate-list").append(certEl);
      } else if(onlyRevoked && cert['revoked'] == true) {
        certEl.classList.add('list-group-item', 'list-group-item-danger');

        $("#revoked-certificate-list").append(certEl);
      }
    });

  } else {
    if(!onlyRevoked) {
      $("#certificate-list").append('No active certificates');
    } else {
      $("#revoked-certificate-list").append('No revoked certificates');
    }
  }
}

$("#certificate-list").ready(function() {
  getUserCertificates();
});

$("#revoked-certificate-list").ready(function() {
  getUserCertificates(true);
});

$(document).ready(function(){
  
  $(this).on('click', '.btn-revoke-certificate', function(event){
    event.preventDefault();
    var cid = $(this).parent().attr('data-cid');
    
    if(cid) {
      fetch(certUri + cid + '/revoke', {
        method: 'PUT', // *GET, POST, PUT, DELETE, etc.
        mode: 'cors', // no-cors, *cors, same-origin
        cache: 'no-cache', // *default, no-cache, reload, force-cache, only-if-cached
        credentials: 'include', // include, *same-origin, omit (browser does not include credentials in the query)
        redirect: 'follow', // manual, *follow, error
        referrerPolicy: 'no-referrer', // no-referrer, *no-referrer-when-downgrade, origin, origin-when-cross-origin, same-origin, strict-origin, strict-origin-when-cross-origin, unsafe-url
      })
      .then(response => response.json())
      .then(data => {
        if(data.success) {
          getUserCertificates();
          getUserCertificates(true);
          _displaySuccess(data.message);
        } else {
          _displayError(data.message);
        }
      })
      .catch(error => {
        _displayError('Unable to revoke certificate');
        console.error('Unable to revoke certificate', error)
      });
    }

  });


  $("#submit-user-info").click(function(event){
      event.preventDefault();
      var body = {
        'FirstName': $("#change-first-name").val().trim(),
        'LastName': $("#change-last-name").val().trim()
      }

      fetch(userUri, {
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
      .then(response => response.json())
      .then(data => {
        if(data.success) {
          getUser();
          scrollTo(0, 0);
          _displaySuccess(data.message);
        } else {
          _displayError(data.message);
        }
      })
      .catch(error => {
        _displayError('Unable to change user info');
        console.error('Unable to change user info', error)
      });
  });

  $("#submit-password-change").click(function(event){
      event.preventDefault();
      var body = {
        'Password': $("#change-password").val().trim(),
        'NewPassword': $("#change-new-password").val().trim()
      };

      fetch(userUri, {
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
      .then(response => response.json())
      .then(data => {
        if(data.success) {
          $("#change-password").val('');
          $("#change-new-password").val('');
          _displaySuccess(data.message);
        } else {
          _displayError(data.message);
        }
      })
      .catch(error => {
        _displayError('Unable to change password');
        console.error('Unable to change password', error)
      });
  });

  $("#create-certificate").click(function(event){
    event.preventDefault();

    fetch(certUri, {
      method: 'POST', // *GET, POST, PUT, DELETE, etc.
      mode: 'cors', // no-cors, *cors, same-origin
      cache: 'no-cache', // *default, no-cache, reload, force-cache, only-if-cached
      credentials: 'include', // include, *same-origin, omit (browser does not include credentials in the query)
      headers: {
        'Content-Type': 'application/json',
      },
      redirect: 'follow', // manual, *follow, error
      referrerPolicy: 'no-referrer', // no-referrer, *no-referrer-when-downgrade, origin, origin-when-cross-origin, same-origin, strict-origin, strict-origin-when-cross-origin, unsafe-url
    })
    .then(response => response.json())
    .then(data => {
      console.log(data);
      if(data.success) {
        getUserCertificates();
        _displaySuccess(data.message);
      } else {
        _displayError(data.message);
      }
    })
    .catch(error => {
      _displayError('Unable to create certificate');
      console.error('Unable to create certificate', error)
    });
  });

  $("a#log-out-button").click(function(){
    try {
        logout(logoutUri);
    }
    catch(error) {
        console.log(error);
    }
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
  window.location = "/login.html";
}

/* onload activity */
window.onload = getUser;



const baseUri = 'https://localhost:5001/';
const getUserUri = baseUri + 'users/tt';

const userDiv = document.getElementById('users');
const userNameHeader = document.getElementById('user-name');
const userEmailHeader = document.getElementById('user-email');

function getUser() {
  fetch(getUserUri)
    .then(response => response.json())
    .then(data => _displayUserInformation(data))
    .catch(error => console.error('Unable to get user information.', error));
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

document.getElementById('submit-user-info').addEventListener('click', function() {
  // TODO: Remove hardcoded userid
  fetch(baseUri + 'users/lb/', {
    method: 'PUT',
    headers: {
      'Content-Type': 'application/json'
    },
    body: JSON.stringify({
      "UserId": "lb",
      "FirstName": "Bruegger",
      "LastName": "Lukas"
    })
  })
  .catch(error => console.error('Unable to update user info', error))
})

// Function calls
getUser();


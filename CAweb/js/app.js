
const baseUri = 'https://localhost:5001/api/';
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

// Function calls
getUser();


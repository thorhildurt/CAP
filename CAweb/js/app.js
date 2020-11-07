//const uri = 'https://randomuser.me/api/?results=10';
const baseUri = 'https://localhost:5001/';
const userDiv = document.getElementById('users');

function getUsers() {
  fetch(baseUri + 'users')
    .then(response => response.json())
    .then(data => _displayUsers(data))
    .catch(error => console.error('Unable to get users.', error));
}

function _displayUsers(data) {
    let users = data
    let mappedUsers = users.map( function(user) {
      let div = createNode('div'), span = createNode('span');
      span.innerHTML = `${user.firstName} ${user.lastName}`;
      append(div, span);
      append(userDiv, div);
    });
    return mappedUsers;
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
      "FirstName": "Bruegger5",
      "LastName": "Lukas"
    })
  })
  .catch(error => console.error('Unable to update user info', error))
})


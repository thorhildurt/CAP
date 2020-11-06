//const uri = 'https://randomuser.me/api/?results=10';
const uri = 'https://localhost:5001/api/users';
const userDiv = document.getElementById('users');

function getUsers() {
  fetch(uri)
    .then(response => response.json())
    .then(data => _displayUsers(data))
    .catch(error => console.error('Unable to get users.', error));
}

function _displayUsers(data) {
    let users = data.results
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


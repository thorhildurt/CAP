const uri = 'https://randomuser.me/api/?results=10';
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
      span.innerHTML = `${user.name.first} ${user.name.last}`;
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


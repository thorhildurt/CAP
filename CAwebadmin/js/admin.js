const baseUri = 'https://localhost:5001/';
const getStatusUri = baseUri + 'monitor/';

const statusDiv = document.getElementById('ca-status');

function getCAStatus() {
    fetch(getStatusUri, {
      method: 'GET',
      credentials: 'include'
    }).then(response => response.json())
    .then(data => _displayCAStatus(data))
    .catch(error => _redirect(error)); 
}

function _displayCAStatus(data) {
    let numCertDiv = createNode('div');
    numCertDiv.innerHTML = `Number of issued certificates: ${data.numberOfIssuedCertificates}`;
    append(statusDiv, numCertDiv);

    let numRevCertDiv = createNode('div');
    numRevCertDiv.innerHTML = `Number of revoked certificates: ${data.numberOfRevokedCertificates}`;
    append(statusDiv, numRevCertDiv);

    let currSerialNumberDiv = createNode('div');
    currSerialNumberDiv.innerHTML = `Current serial number: ${data.currentSerialNumber}`;
    append(statusDiv, currSerialNumberDiv);
}

function createNode(element) {
    return document.createElement(element);
}
  
function append(parent, element) {
    return parent.appendChild(element);
}

/* onload activity */
window.onload = getCAStatus;

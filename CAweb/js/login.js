// credits: https://zellwk.com/blog/frontend-login-system/

const loginButton = document.getElementById('log-in-button');

async function caLogin (userid, password) {
    // zlFetch encaupsulates the fetch function in order return json
    const response = await zlFetch.post(loginEndpoint, {
        // auth transforms usersname and pawwsorrd into basic
        // authentication header
        auth: {
            username: userid, 
            password: password
        },
        body: { }
    })
}
## Certification Authority Project - Team 7
1. Import all .ova files into virtual box
2. Start up the machines in the following order: **backup**, **ca database**, **ca core**, **router**, **ca web server**, **client**
3. Enter the following credentials to log in to the client:
Username: `caclient`
Password: `Nice.Client20`
4. Open Firefox browser and go to **https://192.168.56.5/Home/Login**
5. To login to the system you can use one of the projects predefined users:
- lb D15Licz6
- ps KramBamBuli 
- ms MidbSvlJ
- a3 Astrid
6. Then you should be able to create/revoke certificates, update user information, and password. 
7. Note that the login using certificates is not implemented in the system (explained in the report), thus for reviewing the admin interface go to there is enabled direct access to **https://192.168.56.5/Admin**

## Admin workstation login information
### Admin SSH credentials for all the machines
- **Web Server**
SSH command: `ssh appsec@192.168.56.5` 
Password: `Appsec20 `

- **Firewall/Router**
SSH command: `ssh carouter@192.168.100.9` 
Password: `Give.Me.Route20`

- **CA core**
SSH command: `ssh caadmin@192.168.100.4` 
Password: `Help.Me20`

- **Database server**
SSH command: `ssh cadbadmin@192.168.100.7` 
Password: `Give.Me.Data20`

- **Backup server**
SSH command: `ssh cabackup@192.168.100.10` 
Password: `Need.Backup20`

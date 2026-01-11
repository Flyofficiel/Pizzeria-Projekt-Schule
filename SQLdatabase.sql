drop database pizzaprojekt;
Create database pizzaprojekt;
use pizzaprojekt;
Create table User(
username varchar(50),
passwort varchar(50)
);

insert into User(username,passwort) value
('admin','admin');

select * from User;
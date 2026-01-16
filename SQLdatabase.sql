drop database pizzaprojekt;
Create database pizzaprojekt;
use pizzaprojekt;
Create table User(
username varchar(50),
passwort varchar(50)
);
create table speisen(
speisename varchar(100) unique primary key,
preis double,
zutaten varchar(100)
);
Create table Bestellungen (
    bestellnr INT unique primary key,
    speisename_fk varchar(100), -- Nur diese Spalte wird zur Verknüpfung benötigt
    
    -- Fremdschlüssel nur auf den Namen setzen
    foreign key(speisename_fk) references speisen (speisename)
    
);
insert into speisen (speisename, preis, zutaten) values ('Pizza Margherita', 8.50, 'Tomaten, Käse');
insert into Bestellungen (bestellnr, speisename_fk) values (1, 'Pizza Margherita');

CREATE VIEW Bestellungsübersicht AS
SELECT 
    B.bestellnr, 
    S.speisename, 
    S.preis,
    S.zutaten
FROM Bestellungen B
JOIN speisen S ON B.speisename_fk = S.speisename;

-- Jetzt kannst du ganz einfach abfragen:
SELECT * FROM Bestellungsübersicht;

insert into User(username,passwort) value
('admin','admin');

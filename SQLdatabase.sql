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

create table mitarbeiter(
personalnr int not null primary key,
name varchar(100),
berreich varchar(100)
);

-- 1. Tabelle Tische korrigieren
Create table tische (
    tisch int not null,
    slot int not null,
    datum datetime not null,
    bestellnr_fk int,
    -- Wir machen die Kombination aus Tisch, Slot und Datum zum Primärschlüssel
    primary key (tisch, slot, datum),
    foreign key(bestellnr_fk) references Bestellungen (bestellnr)
);

-- 2. Tabelle Reservierungen korrigieren
create table reservierungen(
    gastname varchar(100) primary key,
    tisch_fk int,
    slot_fk int,
    datum_fk datetime,

    -- Ein Fremdschlüssel, der auf alle drei Spalten gleichzeitig verweist
    foreign key(tisch_fk, slot_fk, datum_fk) references tische (tisch, slot, datum)
);

create table rechnungen(
rechnungsnr int not null unique primary key,
bestellnr_fk int,
gesamtpreis double,

foreign key(bestellnr_fk) references Bestellungen (bestellnr)
);

insert into speisen (speisename, preis, zutaten) values ('Pizza Margherita', 8.50, 'Tomaten, Käse');
insert into Bestellungen (bestellnr, speisename_fk) values (1, 'Pizza Margherita');
insert into speisen (speisename, preis, zutaten) values ('Pizza Salami',8.50,'salami, tomaten, käse');
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

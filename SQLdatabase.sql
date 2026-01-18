drop database pizzaprojekt;
Create database pizzaprojekt;
use pizzaprojekt;
Create table besnutzer(
username varchar(50),
passwort varchar(50)
);
create table speisen(
speisename varchar(100) unique primary key,
preis double,
zutaten varchar(100)
);
create table mitarbeiter(
personalnr int not null primary key,
name varchar(100),
berreich varchar(100)
);
Create table tische (
    tisch int not null,
    slot int not null,
    datum datetime not null,
    lage varchar(50),
    -- Wir machen die Kombination aus Tisch, Slot und Datum zum Primärschlüssel
    primary key (tisch, slot, datum)
);

CREATE TABLE bestellungen (
    bestellnr INT PRIMARY KEY,
    datum DATETIME,
    tisch_fk INT,
    personalnr_fk int,
    foreign key(personalnr_fk) references mitarbeiter (personalnr),
    foreign key(tisch_fk) references tische(tisch)
);

CREATE TABLE bestellposition (
    positionid INT AUTO_INCREMENT PRIMARY KEY,
    bestellnr_fk INT,
    speisename_fk VARCHAR(100),
    menge INT,
    einzelpreis DOUBLE,

    FOREIGN KEY (bestellnr_fk) REFERENCES bestellungen(bestellnr),
    FOREIGN KEY (speisename_fk) REFERENCES speisen(speisename)
);


-- 2. Tabelle Reservierungen korrigieren
create table reservierungen(
    gastname varchar(100) primary key,
    tisch_fk int,
    slot_fk int,
    datum_fk datetime,
    personenanzahl int,

    -- Ein Fremdschlüssel, der auf alle drei Spalten gleichzeitig verweist
    foreign key(tisch_fk, slot_fk, datum_fk) references tische (tisch, slot, datum)
);

create table rechnungen(
rechnungsnr int not null unique primary key,
bestellnr_fk int,
gesamtpreis double,

foreign key(bestellnr_fk) references Bestellungen (bestellnr)
);

SELECT 
    SUM(menge * einzelpreis) AS gesamtumsatz
FROM bestellposition;

SELECT 
    DATE(b.datum) AS tag,
    SUM(p.menge * p.einzelpreis) AS umsatz
FROM bestellungen b
JOIN bestellposition p 
    ON b.bestellnr = p.bestellnr_fk
GROUP BY DATE(b.datum);

SELECT 
    m.name,
    SUM(p.menge * p.einzelpreis) AS umsatz
FROM mitarbeiter m
JOIN bestellungen b 
    ON m.personalnr = b.personalnr_fk
JOIN bestellposition p 
    ON b.bestellnr = p.bestellnr_fk
GROUP BY m.name;

SELECT 
    b.tisch_fk,
    SUM(p.menge * p.einzelpreis) AS umsatz
FROM bestellungen b

JOIN bestellposition p 
    ON b.bestellnr = p.bestellnr_fk
GROUP BY b.tisch_fk;

SELECT 
    s.speisename,
    SUM(p.menge) AS verkauft
FROM speisen s
JOIN bestellposition p
    ON s.speisename = p.speisename_fk
GROUP BY s.speisename
ORDER BY verkauft DESC;

CREATE VIEW UmsatzProTag AS
SELECT 
    DATE(b.datum) AS tag,
    SUM(p.menge * p.einzelpreis) AS umsatz
FROM bestellungen b
JOIN bestellposition p 
ON b.bestellnr = p.bestellnr_fk
GROUP BY DATE(b.datum);



insert into besnutzer(username,passwort) value
('admin','admin');

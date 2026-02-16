drop  database if exists pizzaprojekt;
Create database pizzaprojekt;
use pizzaprojekt;

-- Tabellen

create table speisen(
speise_id int primary key,
speisename varchar(100) unique,
speisentyp varchar(100),
preis double,
zutaten varchar(100),
aktiv boolean default true
);
create table gast(
gastid int auto_increment unique primary key,
gastvorname varchar(100),
gastnachname varchar(100),
telephonenr varchar(20),
aktiv boolean default true

);

create table mitarbeiter(
personalnr int not null primary key,
vorname varchar(100),
nachname varchar(100),
bereich varchar(100),
passwort varchar(100),
rolle varchar(100) ,
aktiv boolean default true
);

Create table tische (
    tisch_id int not null,
    max_personen int,
    aktiv boolean default true,
    lage varchar(100),
    berreich varchar(100),
    primary key (tisch_id)
);

CREATE TABLE bestellungen (
    bestellnr INT auto_increment PRIMARY KEY,
    datum DATETIME,
    gast_id_fk int,
    tisch_id_fk INT,
    personalnr_fk int,
    foreign key(personalnr_fk) references mitarbeiter (personalnr),
    foreign key(tisch_id_fk) references tische(tisch_id),
    foreign key(gast_id_fk) references gast(gastid)
);


CREATE TABLE bestellposition (
    positionid INT AUTO_INCREMENT PRIMARY KEY,
    bestellnr_fk INT,
    speise_id_fk INT, 
    menge INT,
    preis_beim_kauf DOUBLE,

    FOREIGN KEY (bestellnr_fk) REFERENCES bestellungen(bestellnr),
    FOREIGN KEY (speise_id_fk) REFERENCES speisen(speise_id)
);


create table reservierungen(
    reservierungs_id int auto_increment,
    tisch_id_fk int,
    slot int,
    datum datetime ,
    personenanzahl int,
    gastid_fk int,
    zustand varchar(100),
    primary key(reservierungs_id),

    -- Ein Fremdschlüssel, der auf alle drei Spalten gleichzeitig verweist
    foreign key(tisch_id_fk) references tische (tisch_id),
    foreign key(gastid_fk) references gast (gastid),
    
    unique(tisch_id_fk,datum,slot)
);

create table rechnungen(
rechnungsnr int not null unique primary key,
bestellnr_fk int,
gesamtpreis double,

foreign key(bestellnr_fk) references Bestellungen (bestellnr)
);

-- inserts

-- 2er Tische
INSERT INTO tische VALUES
(1,2,true,'Frei','Innen vorne'),(2,2,true,'Frei','Innen vorne'),(3,2,true,'Frei','Innen vorne'),(4,2,true,'Frei','Innen vorne'),(5,2,true,'Frei','Innen vorne'),
(6,2,true,'Frei','Innen vorne'),(7,2,true,'Frei','Innen vorne'),(8,2,true,'Frei','Innen vorne'),(9,2,true,'Frei','Innen vorne'),(10,2,true,'Frei','Innen vorne');

-- 4er Tische
INSERT INTO tische VALUES
(11,4,true,'Frei','Innen hinten'),(12,4,true,'Frei','Innen hinten'),(13,4,true,'Frei','Innen hinten'),(14,4,true,'Frei','Innen hinten'),(15,4,true,'Frei','Innen hinten'),
(16,4,true,'Frei','Innen hinten'),(17,4,true,'Frei','Innen hinten'),(18,4,true,'Frei','Innen hinten'),(19,4,true,'Frei','Innen hinten'),(20,4,true,'Frei','Innen hinten');

-- 6er Tische
INSERT INTO tische VALUES
(21,6,true,'Frei',''),(22,6,true,'Frei',''),(23,6,true,'Frei',''),(24,6,true,'Frei',''),(25,6,true,'Frei',''),
(26,6,true,'Frei',''),(27,6,true,'Frei',''),(28,6,true,'Frei',''),(29,6,true,'Frei',''),(30,6,true,'Frei','');

-- 8er Tische
INSERT INTO tische VALUES
(31,8,true,'Frei','Terrasse'),(32,8,true,'Frei','Terrasse'),(33,8,true,'Frei','Terrasse'),(34,8,true,'Frei','Terrasse'),(35,8,true,'Frei','Terrasse');

-- 10er Tische
INSERT INTO tische VALUES
(36,10,true,'Frei','VIP / Gruppen'),(37,10,true,'Frei','VIP / Gruppen'),(38,10,true,'Frei','VIP / Gruppen'),(39,10,true,'Frei','VIP / Gruppen'),(40,10,true,'Frei','VIP / Gruppen');
   
INSERT INTO speisen (speise_id,speisename,speisentyp, preis, zutaten,aktiv) VALUES
-- 🍕 PIZZA
(1,'Pizza Margherita','🍕 PIZZA', 8.50, 'Tomatensauce, Mozzarella',true),
(2,'Pizza Salami','🍕 PIZZA', 9.50, 'Tomatensauce, Mozzarella, Salami',true),
(3,'Pizza Prosciutto','🍕 PIZZA', 10.00, 'Tomatensauce, Mozzarella, Schinken',true),
(4,'Pizza Funghi','🍕 PIZZA', 9.00, 'Tomatensauce, Mozzarella, Champignons',true),
(5,'Pizza Hawaii','🍕 PIZZA', 10.50, 'Schinken, Ananas, Käse',true),
(6,'Pizza Tonno','🍕 PIZZA', 11.00, 'Thunfisch, Zwiebeln, Käse',true),
(7,'Pizza Quattro Formaggi','🍕 PIZZA', 11.50, '4 Käsesorten',true),
(8,'Pizza Vegetaria','🍕 PIZZA', 10.00, 'Gemüse, Käse',true),

-- 🍝 PASTA
(9,'Pasta Bolognese','🍝 PASTA', 11.50, 'Rinderhack, Tomatensauce',true),
(10,'Pasta Carbonara','🍝 PASTA', 12.00, 'Sahnesauce, Ei, Speck',true),
(11,'Pasta Napoli','🍝 PASTA', 9.50, 'Tomatensauce',true),
(12,'Pasta Alfredo','🍝 PASTA', 12.50, 'Sahnesauce, Hähnchen',true),

-- 🥗 SALATE
(13,'Insalata Mista','🥗 SALATE', 6.50, 'Salat, Tomaten, Gurken',true),
(14,'Caesar Salad','🥗 SALATE', 9.00, 'Hähnchen, Parmesan, Croutons',true),

-- 🥤 GETRÄNKE
(15,'Cola 0,33l','🥤 GETRÄNKE', 3.00, '',true),
(16,'Cola Zero 0,33l','🥤 GETRÄNKE', 3.00, '',true),
(17,'Fanta 0,33l','🥤 GETRÄNKE', 3.00, '',true),
(18,'Sprite 0,33l','🥤 GETRÄNKE', 3.00, '',true),
(19,'Mineralwasser 0,5l','🥤 GETRÄNKE', 2.50, '',true),
(20,'Apfelschorle 0,5l','🥤 GETRÄNKE', 3.00, '',true),

-- 🍰 DESSERT
(21,'Tiramisu','🍰 DESSERT', 5.00, 'Mascarpone, Kaffee',true),
(22,'Panna Cotta','🍰 DESSERT', 4.50, 'Sahne, Vanille',true),
(23,'Schokoladenkuchen','🍰 DESSERT', 4.00, 'Schokolade',true);

-- Mitarbeiter inserts

INSERT INTO mitarbeiter (personalnr, vorname,nachname, bereich, passwort,rolle, aktiv) VALUES
(0, 'Luigi',' Rossi', 'CEO','fdgfgdjsdhf','Verwaltung',true),
(1, 'Marco',' Habibi', 'service','jsdhf','service',true),
(2, 'Giulia',' Bianchi', 'küche','kjdfsnklsf','koch',true),
(3, 'Luca',' Romano', 'service','ösldkfsk','service',true),
(4, 'Sara',' Conti', 'Kasse','lödgkdlöfgv','service',true),
(5, 'Antonio',' Greco', 'Küche','pdfglokjdsp','koch',true),
(6, 'Elena',' Ferrari', 'service','poigvjk','service',true),
(7,'Lucas',' Huber','EDV Admin','admin1','Verwaltung',true),
(8,'diaa','','EDV Admin','admin2','Verwaltung',true),
(9,'Julian','','EDV Admin','admin3','Verwaltung',true);





-- berechnungen

-- Umsatz berechnen

-- 1. Alte View löschen, falls sie existiert
DROP VIEW IF EXISTS UmsatzProTag;

-- 2. View mit dem neuen Spaltennamen 'preis_beim_kauf' erstellen
CREATE VIEW UmsatzProTag AS
SELECT 
    DATE(b.datum) AS tag,
    SUM(p.menge * p.preis_beim_kauf) AS umsatz
FROM bestellungen b
JOIN bestellposition p ON b.bestellnr = p.bestellnr_fk
GROUP BY DATE(b.datum);

-- 3. Umsatz pro Mitarbeiter (ebenfalls korrigiert)
SELECT 
    m.vorname,
    SUM(p.menge * p.preis_beim_kauf) AS umsatz
FROM mitarbeiter m
JOIN bestellungen b ON m.personalnr = b.personalnr_fk
JOIN bestellposition p ON b.bestellnr = p.bestellnr_fk
GROUP BY m.vorname;

-- 3. Abfrage: Beliebteste Speisen (korrigiert auf ID-Join)
SELECT 
    s.speisename,
    SUM(p.menge) AS verkauft
FROM speisen s
JOIN bestellposition p ON s.speise_id = p.speise_id_fk
GROUP BY s.speisename
ORDER BY verkauft DESC;

-- umsatz pro woche

DROP VIEW IF EXISTS UmsatzProWoche;

CREATE VIEW UmsatzProWoche AS
SELECT 
    YEAR(b.datum) AS jahr,
    WEEK(b.datum, 1) AS kalenderwoche,   -- ISO-Woche (Mo–So)
    SUM(p.menge * p.preis_beim_kauf) AS umsatz
FROM bestellungen b
JOIN bestellposition p 
    ON b.bestellnr = p.bestellnr_fk
GROUP BY 
    YEAR(b.datum),
    WEEK(b.datum, 1);
    
-- pro monat

DROP VIEW IF EXISTS UmsatzProMonat;

CREATE VIEW UmsatzProMonat AS
SELECT 
    YEAR(b.datum) AS jahr,
    MONTH(b.datum) AS monat,
    SUM(p.menge * p.preis_beim_kauf) AS umsatz
FROM bestellungen b
JOIN bestellposition p 
    ON b.bestellnr = p.bestellnr_fk
GROUP BY 
    YEAR(b.datum),
    MONTH(b.datum);
    
-- pro gast

DROP VIEW IF EXISTS UmsatzProGast;

CREATE VIEW UmsatzProGast AS
SELECT 
    g.gastid,
    g.gastvorname,
    g.gastnachname,
    SUM(p.menge * p.preis_beim_kauf) AS umsatz
FROM gast g
JOIN bestellungen b 
    ON g.gastid = b.gast_id_fk -- Hier war der Fehler: gast_id_fk statt gastid_fk
JOIN bestellposition p 
    ON b.bestellnr = p.bestellnr_fk
GROUP BY 
    g.gastid, 
    g.gastvorname, 
    g.gastnachname;

-- test

UPDATE tische
SET lage = 'Frei'
WHERE tisch_id >= 1;
UPDATE tische
SET lage = 'Reserviert'
WHERE tisch_id = 2;

UPDATE tische
SET lage = 'Besetzt'
WHERE tisch_id = 3;
SELECT tisch_id, lage
FROM tische
ORDER BY tisch_id;

ALTER TABLE rechnungen
MODIFY rechnungsnr INT AUTO_INCREMENT;

ALTER TABLE rechnungen
ADD COLUMN datum DATETIME,
ADD COLUMN zahlungsart VARCHAR(20),
ADD COLUMN trinkgeld DOUBLE;

-- Mysql Workbench ausgabe

select * from speisen;

select * from mitarbeiter;
select* from tische;
select* from reservierungen;
select * from bestellposition;


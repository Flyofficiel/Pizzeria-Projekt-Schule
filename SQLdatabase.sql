drop  database if exists pizzaprojekt;
Create database pizzaprojekt;
use pizzaprojekt;

-- Tabellen

create table speisen(
speise_id int primary key,
speisename varchar(100) unique,
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
aktiv boolean default true
);

Create table tische (
    tisch_id int not null,
    max_personen int,
    aktiv boolean default true,
    lage varchar(100),
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
(1,2,true,'Frei'),(2,2,true,'Frei'),(3,2,true,'Frei'),(4,2,true,'Frei'),(5,2,true,'Frei'),
(6,2,true,'Frei'),(7,2,true,'Frei'),(8,2,true,'Frei'),(9,2,true,'Frei'),(10,2,true,'Frei');

-- 4er Tische
INSERT INTO tische VALUES
(11,4,true,'Frei'),(12,4,true,'Frei'),(13,4,true,'Frei'),(14,4,true,'Frei'),(15,4,true,'Frei'),
(16,4,true,'Frei'),(17,4,true,'Frei'),(18,4,true,'Frei'),(19,4,true,'Frei'),(20,4,true,'Frei');

-- 6er Tische
INSERT INTO tische VALUES
(21,6,true,'Frei'),(22,6,true,'Frei'),(23,6,true,'Frei'),(24,6,true,'Frei'),(25,6,true,'Frei'),
(26,6,true,'Frei'),(27,6,true,'Frei'),(28,6,true,'Frei'),(29,6,true,'Frei'),(30,6,true,'Frei');

-- 8er Tische
INSERT INTO tische VALUES
(31,8,true,'Frei'),(32,8,true,'Frei'),(33,8,true,'Frei'),(34,8,true,'Frei'),(35,8,true,'Frei');

-- 10er Tische
INSERT INTO tische VALUES
(36,10,true,'Frei'),(37,10,true,'Frei'),(38,10,true,'Frei'),(39,10,true,'Frei'),(40,10,true,'Frei');

INSERT INTO speisen (speise_id,speisename, preis, zutaten,aktiv) VALUES
-- 🍕 PIZZA
(1,'Pizza Margherita', 8.50, 'Tomatensauce, Mozzarella',true),
(2,'Pizza Salami', 9.50, 'Tomatensauce, Mozzarella, Salami',true),
(3,'Pizza Prosciutto', 10.00, 'Tomatensauce, Mozzarella, Schinken',true),
(4,'Pizza Funghi', 9.00, 'Tomatensauce, Mozzarella, Champignons',true),
(5,'Pizza Hawaii', 10.50, 'Schinken, Ananas, Käse',true),
(6,'Pizza Tonno', 11.00, 'Thunfisch, Zwiebeln, Käse',true),
(7,'Pizza Quattro Formaggi', 11.50, '4 Käsesorten',true),
(8,'Pizza Vegetaria', 10.00, 'Gemüse, Käse',true),

-- 🍝 PASTA
(9,'Pasta Bolognese', 11.50, 'Rinderhack, Tomatensauce',true),
(10,'Pasta Carbonara', 12.00, 'Sahnesauce, Ei, Speck',true),
(11,'Pasta Napoli', 9.50, 'Tomatensauce',true),
(12,'Pasta Alfredo', 12.50, 'Sahnesauce, Hähnchen',true),

-- 🥗 SALATE
(13,'Insalata Mista', 6.50, 'Salat, Tomaten, Gurken',true),
(14,'Caesar Salad', 9.00, 'Hähnchen, Parmesan, Croutons',true),

-- 🥤 GETRÄNKE
(15,'Cola 0,33l', 3.00, '',true),
(16,'Cola Zero 0,33l', 3.00, '',true),
(17,'Fanta 0,33l', 3.00, '',true),
(18,'Sprite 0,33l', 3.00, '',true),
(19,'Mineralwasser 0,5l', 2.50, '',true),
(20,'Apfelschorle 0,5l', 3.00, '',true),

-- 🍰 DESSERT
(21,'Tiramisu', 5.00, 'Mascarpone, Kaffee',true),
(22,'Panna Cotta', 4.50, 'Sahne, Vanille',true),
(23,'Schokoladenkuchen', 4.00, 'Schokolade',true);

INSERT INTO mitarbeiter (personalnr, vorname,nachname, bereich, passwort, aktiv) VALUES
(1, 'Marco',' Rossi', 'tisch 1','jsdhf',true),
(2, 'Giulia',' Bianchi', 'küche','kjdfsnklsf',true),
(3, 'Luca',' Romano', 'Tisch 2','ösldkfsk',true),
(4, 'Sara',' Conti', 'Kasse','lödgkdlöfgv',true),
(5, 'Antonio',' Greco', 'Küche','pdfglokjdsp',true),
(6, 'Elena',' Ferrari', 'Tisch 3','poigvjk',true),
(7,'Lucas',' Huber','EDV Admin','admin1',true),
(8,'diaa','','EDV Admin','admin2',true),
(9,'Julian','','EDV Admin','admin3',true);



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

select * from speisen;

select * from mitarbeiter;
select* from tische;
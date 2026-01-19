drop  database if exists pizzaprojekt;
Create database pizzaprojekt;
use pizzaprojekt;

-- Tabellen

create table speisen(
speise_id int unique primary key,
speisename varchar(100) unique,
preis double,
zutaten varchar(100)
);

create table mitarbeiter(
personalnr int not null primary key,
name varchar(100),
berreich varchar(100),
passwort varchar(100)
);

Create table tische (
    tisch_id int not null,
    max_personen int,
    aktiv boolean default true,
    primary key (tisch_id)
);

CREATE TABLE bestellungen (
    bestellnr INT PRIMARY KEY,
    datum DATETIME,
    tisch_id_fk INT,
    personalnr_fk int,
    foreign key(personalnr_fk) references mitarbeiter (personalnr),
    foreign key(tisch_id_fk) references tische(tisch_id)
);

DROP TABLE IF EXISTS bestellposition;
CREATE TABLE bestellposition (
    positionid INT AUTO_INCREMENT PRIMARY KEY,
    bestellnr_fk INT,
    speise_id_fk INT, 
    menge INT,
    preis_beim_kauf DOUBLE, -- Hier wurde aus preis_fk -> preis_beim_kauf

    FOREIGN KEY (bestellnr_fk) REFERENCES bestellungen(bestellnr),
    FOREIGN KEY (speise_id_fk) REFERENCES speisen(speise_id)
);



create table reservierungen(
    gastname varchar(100),
    tisch_id_fk int,
    slot int,
    datum datetime ,
    personenanzahl int,
    telephonnunmmr int,
    primary key(telephonnunmmr,datum,slot),

    -- Ein Fremdschlüssel, der auf alle drei Spalten gleichzeitig verweist
    foreign key(tisch_id_fk) references tische (tisch_id)
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
(1,2,true),(2,2,true),(3,2,true),(4,2,true),(5,2,true),
(6,2,true),(7,2,true),(8,2,true),(9,2,true),(10,2,true);

-- 4er Tische
INSERT INTO tische VALUES
(11,4,true),(12,4,true),(13,4,true),(14,4,true),(15,4,true),
(16,4,true),(17,4,true),(18,4,true),(19,4,true),(20,4,true);

-- 6er Tische
INSERT INTO tische VALUES
(21,6,true),(22,6,true),(23,6,true),(24,6,true),(25,6,true),
(26,6,true),(27,6,true),(28,6,true),(29,6,true),(30,6,true);

-- 8er Tische
INSERT INTO tische VALUES
(31,8,true),(32,8,true),(33,8,true),(34,8,true),(35,8,true);

-- 10er Tische
INSERT INTO tische VALUES
(36,10,true),(37,10,true),(38,10,true),(39,10,true),(40,10,true);

INSERT INTO speisen (speise_id,speisename, preis, zutaten) VALUES
-- 🍕 PIZZA
(1,'Pizza Margherita', 8.50, 'Tomatensauce, Mozzarella'),
(2,'Pizza Salami', 9.50, 'Tomatensauce, Mozzarella, Salami'),
(3,'Pizza Prosciutto', 10.00, 'Tomatensauce, Mozzarella, Schinken'),
(4,'Pizza Funghi', 9.00, 'Tomatensauce, Mozzarella, Champignons'),
(5,'Pizza Hawaii', 10.50, 'Schinken, Ananas, Käse'),
(6,'Pizza Tonno', 11.00, 'Thunfisch, Zwiebeln, Käse'),
(7,'Pizza Quattro Formaggi', 11.50, '4 Käsesorten'),
(8,'Pizza Vegetaria', 10.00, 'Gemüse, Käse'),

-- 🍝 PASTA
(9,'Pasta Bolognese', 11.50, 'Rinderhack, Tomatensauce'),
(10,'Pasta Carbonara', 12.00, 'Sahnesauce, Ei, Speck'),
(11,'Pasta Napoli', 9.50, 'Tomatensauce'),
(12,'Pasta Alfredo', 12.50, 'Sahnesauce, Hähnchen'),

-- 🥗 SALATE
(13,'Insalata Mista', 6.50, 'Salat, Tomaten, Gurken'),
(14,'Caesar Salad', 9.00, 'Hähnchen, Parmesan, Croutons'),

-- 🥤 GETRÄNKE
(15,'Cola 0,33l', 3.00, ''),
(16,'Cola Zero 0,33l', 3.00, ''),
(17,'Fanta 0,33l', 3.00, ''),
(18,'Sprite 0,33l', 3.00, ''),
(19,'Mineralwasser 0,5l', 2.50, ''),
(20,'Apfelschorle 0,5l', 3.00, ''),

-- 🍰 DESSERT
(21,'Tiramisu', 5.00, 'Mascarpone, Kaffee'),
(22,'Panna Cotta', 4.50, 'Sahne, Vanille'),
(23,'Schokoladenkuchen', 4.00, 'Schokolade');

INSERT INTO mitarbeiter (personalnr, name, berreich, passwort) VALUES
(1, 'Marco Rossi', 'tisch 1','jsdhf'),
(2, 'Giulia Bianchi', 'küche','kjdfsnklsf'),
(3, 'Luca Romano', 'Tisch 2','ösldkfsk'),
(4, 'Sara Conti', 'Kasse','lödgkdlöfgv'),
(5, 'Antonio Greco', 'Küche','pdfglokjdsp'),
(6, 'Elena Ferrari', 'Tisch 3','poigvjk'),
(7,'Lucas Huber','EDV Admin','admin1'),
(8,'diaa','EDV Admin','admin2'),
(9,'Julian','EDV Admin','admin3');



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
    m.name,
    SUM(p.menge * p.preis_beim_kauf) AS umsatz
FROM mitarbeiter m
JOIN bestellungen b ON m.personalnr = b.personalnr_fk
JOIN bestellposition p ON b.bestellnr = p.bestellnr_fk
GROUP BY m.name;

DROP VIEW IF EXISTS UmsatzProTag;
CREATE VIEW UmsatzProTag AS
SELECT 
    DATE(b.datum) AS tag,
    SUM(p.menge * p.preis_beim_kauf) AS umsatz -- preis_beim_kauf nutzen!
FROM bestellungen b
JOIN bestellposition p ON b.bestellnr = p.bestellnr_fk
GROUP BY DATE(b.datum);

-- 3. Abfrage: Beliebteste Speisen (korrigiert auf ID-Join)
SELECT 
    s.speisename,
    SUM(p.menge) AS verkauft
FROM speisen s
JOIN bestellposition p ON s.speise_id = p.speise_id_fk
GROUP BY s.speisename
ORDER BY verkauft DESC;

select * from speisen;

select * from mitarbeiter
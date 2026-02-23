drop  database if exists pizzaprojekt;
Create database pizzaprojekt;
use pizzaprojekt;

-- Tabellen

create table speisen(
speise_id INT AUTO_INCREMENT PRIMARY KEY,
speisename varchar(100) unique,
speisentyp varchar(100),
preis DECIMAL(10,2),

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
INSERT INTO gast (gastvorname, gastnachname, telephonenr, aktiv)
VALUES ('Laufkunde', 'Ohne Registrierung', NULL, true);

create table mitarbeiter(
personalnr int not null primary key,
vorname varchar(100),
nachname varchar(100),
bereich varchar(100),
passwort varchar(100),
rolle varchar(100) ,
aktiv boolean default true



);
ALTER TABLE mitarbeiter 
MODIFY rolle ENUM(
'service',
'koch',
'kasse',
'admin',
'management'
);
ALTER TABLE mitarbeiter 
MODIFY bereich ENUM(
'Innen vorne',
'Innen hinten',
'Terrasse',
'Terrasse groß',
'VIP / Gruppen',
'Küche',
'Kasse',
'EDV',
'Management'
);
ALTER TABLE mitarbeiter 
MODIFY personalnr INT AUTO_INCREMENT;
ALTER TABLE mitarbeiter 
MODIFY bereich ENUM(
'Tische 1-10',
'Tische 11-20',
'Tische 21-30',
'Tische 31-40',
'Küche',
'Kasse',
'Management',
'EDV'
);





CREATE TABLE tische (
    tisch_id INT NOT NULL PRIMARY KEY,
    max_personen INT,
    aktiv BOOLEAN DEFAULT true,
    bereich ENUM(
        'Tische 1-10',
        'Tische 11-20',
        'Tische 21-30',
        'Tische 31-40'
    ),
    lage VARCHAR(50) DEFAULT 'Frei'
);








CREATE TABLE bestellungen (
    bestellnr INT auto_increment PRIMARY KEY,
    datum DATETIME,
    gast_id_fk int,
    tisch_id_fk INT,
    personalnr_fk int,
        status VARCHAR(20) DEFAULT 'offen',

    foreign key(personalnr_fk) references mitarbeiter (personalnr),
    foreign key(tisch_id_fk) references tische(tisch_id),
    foreign key(gast_id_fk) references gast(gastid)
);
ALTER TABLE bestellungen
MODIFY gast_id_fk INT NOT NULL;
ALTER TABLE bestellungen
MODIFY datum DATETIME DEFAULT CURRENT_TIMESTAMP;
ALTER TABLE bestellungen 
MODIFY status ENUM('offen','bezahlt','storniert') DEFAULT 'offen';

ALTER table bestellungen ADD slot int;

CREATE TABLE bestellposition (
    positionid INT AUTO_INCREMENT PRIMARY KEY,
    bestellnr_fk INT,
    speise_id_fk INT, 
    menge INT,
    preis_beim_kauf DECIMAL(10,2),

    FOREIGN KEY (bestellnr_fk) 
        REFERENCES bestellungen(bestellnr)
        ON DELETE CASCADE,

    FOREIGN KEY (speise_id_fk) 
        REFERENCES speisen(speise_id)
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
ALTER TABLE reservierungen 
MODIFY zustand ENUM('offen','abgeschlossen','storniert','aktiv');


CREATE TABLE rechnungen(
    rechnungsnr INT AUTO_INCREMENT PRIMARY KEY,
    bestellnr_fk INT,
    
    datum DATETIME,
    zahlungsart VARCHAR(50),
    gesamtpreis DECIMAL(10,2),
trinkgeld DECIMAL(10,2),

    FOREIGN KEY (bestellnr_fk) REFERENCES bestellungen(bestellnr)
);
INSERT INTO tische (tisch_id, max_personen, aktiv, bereich, lage) VALUES

-- 1-10
(1,2,true,'Tische 1-10','Frei'),
(2,2,true,'Tische 1-10','Frei'),
(3,2,true,'Tische 1-10','Frei'),
(4,2,true,'Tische 1-10','Frei'),
(5,2,true,'Tische 1-10','Frei'),
(6,2,true,'Tische 1-10','Frei'),
(7,2,true,'Tische 1-10','Frei'),
(8,2,true,'Tische 1-10','Frei'),
(9,2,true,'Tische 1-10','Frei'),
(10,2,true,'Tische 1-10','Frei'),

-- 11-20
(11,4,true,'Tische 11-20','Frei'),
(12,4,true,'Tische 11-20','Frei'),
(13,4,true,'Tische 11-20','Frei'),
(14,4,true,'Tische 11-20','Frei'),
(15,4,true,'Tische 11-20','Frei'),
(16,4,true,'Tische 11-20','Frei'),
(17,4,true,'Tische 11-20','Frei'),
(18,4,true,'Tische 11-20','Frei'),
(19,4,true,'Tische 11-20','Frei'),
(20,4,true,'Tische 11-20','Frei'),

-- 21-30
(21,6,true,'Tische 21-30','Frei'),
(22,6,true,'Tische 21-30','Frei'),
(23,6,true,'Tische 21-30','Frei'),
(24,6,true,'Tische 21-30','Frei'),
(25,6,true,'Tische 21-30','Frei'),
(26,6,true,'Tische 21-30','Frei'),
(27,6,true,'Tische 21-30','Frei'),
(28,6,true,'Tische 21-30','Frei'),
(29,6,true,'Tische 21-30','Frei'),
(30,6,true,'Tische 21-30','Frei'),

-- 31-40
(31,8,true,'Tische 31-40','Frei'),
(32,8,true,'Tische 31-40','Frei'),
(33,8,true,'Tische 31-40','Frei'),
(34,8,true,'Tische 31-40','Frei'),
(35,8,true,'Tische 31-40','Frei'),
(36,10,true,'Tische 31-40','Frei'),
(37,10,true,'Tische 31-40','Frei'),
(38,10,true,'Tische 31-40','Frei'),
(39,10,true,'Tische 31-40','Frei'),
(40,10,true,'Tische 31-40','Frei');





INSERT INTO speisen (speisename,speisentyp, preis, zutaten,aktiv) VALUES
-- 🍕 PIZZA
('Pizza Margherita','🍕 PIZZA', 8.50, 'Tomatensauce, Mozzarella',true),
('Pizza Salami','🍕 PIZZA', 9.50, 'Tomatensauce, Mozzarella, Salami',true),
('Pizza Prosciutto','🍕 PIZZA', 10.00, 'Tomatensauce, Mozzarella, Schinken',true),
('Pizza Funghi','🍕 PIZZA', 9.00, 'Tomatensauce, Mozzarella, Champignons',true),
('Pizza Hawaii','🍕 PIZZA', 10.50, 'Schinken, Ananas, Käse',true),
('Pizza Tonno','🍕 PIZZA', 11.00, 'Thunfisch, Zwiebeln, Käse',true),
('Pizza Quattro Formaggi','🍕 PIZZA', 11.50, '4 Käsesorten',true),
('Pizza Vegetaria','🍕 PIZZA', 10.00, 'Gemüse, Käse',true),

-- 🍝 PASTA
('Pasta Bolognese','🍝 PASTA', 11.50, 'Rinderhack, Tomatensauce',true),
('Pasta Carbonara','🍝 PASTA', 12.00, 'Sahnesauce, Ei, Speck',true),
('Pasta Napoli','🍝 PASTA', 9.50, 'Tomatensauce',true),
('Pasta Alfredo','🍝 PASTA', 12.50, 'Sahnesauce, Hähnchen',true),

-- 🥗 SALATE
('Insalata Mista','🥗 SALATE', 6.50, 'Salat, Tomaten, Gurken',true),
('Caesar Salad','🥗 SALATE', 9.00, 'Hähnchen, Parmesan, Croutons',true),

-- 🥤 GETRÄNKE
('Cola 0,33l','🥤 GETRÄNKE', 3.00, '',true),
('Cola Zero 0,33l','🥤 GETRÄNKE', 3.00, '',true),
('Fanta 0,33l','🥤 GETRÄNKE', 3.00, '',true),
('Sprite 0,33l','🥤 GETRÄNKE', 3.00, '',true),
('Mineralwasser 0,5l','🥤 GETRÄNKE', 2.50, '',true),
('Apfelschorle 0,5l','🥤 GETRÄNKE', 3.00, '',true),

-- 🍰 DESSERT
('Tiramisu','🍰 DESSERT', 5.00, 'Mascarpone, Kaffee',true),
('Panna Cotta','🍰 DESSERT', 4.50, 'Sahne, Vanille',true),
('Schokoladenkuchen','🍰 DESSERT', 4.00, 'Schokolade',true);

-- Mitarbeiter inserts

INSERT INTO mitarbeiter (vorname,nachname, bereich, passwort,rolle, aktiv) VALUES

('Luigi','Rossi','Management','fdgfgdjsdhf','management',true),

('Marco','Habibi','Tische 1-10','jsdhf','service',true),
('Luca','Romano','Tische 11-20','ösldkfsk','service',true),
('Elena','Ferrari','Tische 21-30','poigvjk','service',true),
('Mario','Test','Tische 31-40','test123','service',true),

('Giulia','Bianchi','Küche','kjdfsnklsf','koch',true),
('Antonio','Greco','Küche','pdfglokjdsp','koch',true),

('Sara','Conti','Kasse','lödgkdlöfgv','kasse',true),

('Lucas','Huber','EDV','admin1','admin',true),
('Diaa','','EDV','admin2','admin',true),
('Julian','','EDV','admin3','admin',true);






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
    CONCAT(g.gastvorname, ' ', g.gastnachname) AS gastname,
    SUM(p.menge * p.preis_beim_kauf) AS umsatz
FROM gast g
JOIN bestellungen b 
    ON g.gastid = b.gast_id_fk
JOIN bestellposition p 
    ON b.bestellnr = p.bestellnr_fk
GROUP BY 
    g.gastid, gastname
ORDER BY umsatz DESC;

-- Mysql Workbench ausgabe

select * from speisen;

select * from mitarbeiter;
select* from tische;
select* from reservierungen;
select * from bestellposition;
DESCRIBE bestellposition;
DESCRIBE rechnungen;
SELECT * FROM bestellungen;
SELECT * FROM bestellposition;
SELECT * FROM rechnungen;
SELECT * FROM bestellungen;
SELECT bestellnr, status FROM bestellungen;

UPDATE bestellungen
SET status = 'bezahlt'
WHERE bestellnr = @bnr;


SELECT * FROM tische WHERE lage = 'Besetzt';
SELECT personalnr, bereich FROM mitarbeiter WHERE rolle = 'service';
SELECT DISTINCT bereich FROM tische;
DESCRIBE tische;
SELECT personalnr, bereich 
FROM mitarbeiter 
WHERE personalnr = 2;
SELECT tisch_id, bereich
FROM tische
WHERE bereich = 'Tische 1-10';
SELECT * FROM gast;
SELECT bestellnr, tisch_id_fk, datum, slot, status
FROM bestellungen
ORDER BY bestellnr DESC;
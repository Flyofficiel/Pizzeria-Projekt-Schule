-- ############################################################
-- DATENBANK ABGABE: PIZZERIA PROJEKT
-- ############################################################

DROP DATABASE IF EXISTS pizzaprojekt;
CREATE DATABASE pizzaprojekt;
USE pizzaprojekt;

-- ------------------------------------------------------------
-- TABELLEN ERSTELLEN
-- ------------------------------------------------------------

-- Hier speichern wir unsere Speisekarte
CREATE TABLE speisen(
    speise_id INT AUTO_INCREMENT PRIMARY KEY,
    speisename VARCHAR(100) UNIQUE,
    speisentyp VARCHAR(100),
    preis DECIMAL(10,2),
    zutaten VARCHAR(100),
    aktiv BOOLEAN DEFAULT TRUE
);

-- Tabelle für die Kunden (Gäste)
CREATE TABLE gast(
    gastid INT AUTO_INCREMENT PRIMARY KEY,
    gastvorname VARCHAR(100),
    gastnachname VARCHAR(100),
    telephonenr VARCHAR(20),
    aktiv BOOLEAN DEFAULT TRUE,
    laufgast BOOLEAN DEFAULT FALSE
);

-- Hier legen wir den Standard-Laufkunden an
INSERT INTO gast (gastvorname, gastnachname, telephonenr, aktiv, laufgast)
VALUES ('Laufkunde', 'Ohne Registrierung', NULL, TRUE, TRUE);

-- Personalverwaltung mit Rollen und Bereichen
CREATE TABLE mitarbeiter(
    personalnr INT AUTO_INCREMENT PRIMARY KEY,
    vorname VARCHAR(100),
    nachname VARCHAR(100),
    bereich ENUM(
        'Tische 1-10', 'Tische 11-20', 'Tische 21-30', 'Tische 31-40',
        'Küche', 'Kasse', 'EDV', 'Management'
    ) NOT NULL,
    passwort VARCHAR(100),
    rolle ENUM('service', 'koch', 'kasse', 'admin', 'management'),
    aktiv BOOLEAN DEFAULT TRUE
);

-- Anpassungen für die Mitarbeiter-Tabelle (längere Texte erlauben)
ALTER TABLE mitarbeiter MODIFY COLUMN bereich VARCHAR(255);
ALTER TABLE mitarbeiter MODIFY COLUMN rolle VARCHAR(255);
ALTER TABLE mitarbeiter MODIFY COLUMN vorname VARCHAR(255);

-- Unsere 40 Tische im Restaurant
CREATE TABLE tische (
    tisch_id INT NOT NULL PRIMARY KEY,
    max_personen INT,
    aktiv BOOLEAN DEFAULT TRUE,
    bereich ENUM('Tische 1-10', 'Tische 11-20', 'Tische 21-30', 'Tische 31-40'),
    lage VARCHAR(50) DEFAULT 'Frei',
    ort VARCHAR(15) DEFAULT 'Saal'
);

-- Die Haupttabelle für Bestellungen
CREATE TABLE bestellungen (
    bestellnr INT AUTO_INCREMENT PRIMARY KEY,
    datum DATETIME,
    gast_id_fk INT,
    tisch_id_fk INT,
    personalnr_fk INT,
    status VARCHAR(20) DEFAULT 'offen',
    FOREIGN KEY(personalnr_fk) REFERENCES mitarbeiter (personalnr),
    FOREIGN KEY(tisch_id_fk) REFERENCES tische(tisch_id),
    FOREIGN KEY(gast_id_fk) REFERENCES gast(gastid)
);

-- Nachträgliche Änderungen an der Bestellung-Tabelle
ALTER TABLE bestellungen MODIFY gast_id_fk INT NOT NULL;
ALTER TABLE bestellungen MODIFY datum DATETIME DEFAULT CURRENT_TIMESTAMP;
ALTER TABLE bestellungen MODIFY status ENUM('offen','bezahlt','storniert') DEFAULT 'offen';
ALTER TABLE bestellungen ADD slot INT;

-- Die einzelnen Gerichte pro Bestellung
CREATE TABLE bestellposition (
    positionid INT AUTO_INCREMENT PRIMARY KEY,
    bestellnr_fk INT,
    speise_id_fk INT, 
    menge INT,
    preis_beim_kauf DECIMAL(10,2),
    FOREIGN KEY (bestellnr_fk) REFERENCES bestellungen(bestellnr) ON DELETE CASCADE,
    FOREIGN KEY (speise_id_fk) REFERENCES speisen(speise_id)
);

-- Tisch-Reservierungen mit Slot-System
CREATE TABLE reservierungen(
    reservierungs_id INT AUTO_INCREMENT PRIMARY KEY,
    tisch_id_fk INT,
    slot INT,
    datum DATETIME,
    personenanzahl INT,
    gastid_fk INT,
    zustand VARCHAR(100),
    FOREIGN KEY(tisch_id_fk) REFERENCES tische (tisch_id),
    FOREIGN KEY(gastid_fk) REFERENCES gast (gastid),
    UNIQUE(tisch_id_fk, datum, slot)
);

-- Anpassungen für die Reservierungen
ALTER TABLE reservierungen MODIFY zustand ENUM('offen','abgeschlossen','storniert','aktiv');
ALTER TABLE reservierungen MODIFY COLUMN zustand VARCHAR(20);

-- Abrechnungen/Quittungen
CREATE TABLE rechnungen(
    rechnungsnr INT AUTO_INCREMENT PRIMARY KEY,
    bestellnr_fk INT,
    datum DATETIME,
    zahlungsart VARCHAR(50),
    gesamtpreis DECIMAL(10,2),
    trinkgeld DECIMAL(10,2),
    FOREIGN KEY (bestellnr_fk) REFERENCES bestellungen(bestellnr)
);

-- ------------------------------------------------------------
-- DATEN EINFÜGEN (Tische, Speisen, Mitarbeiter)
-- ------------------------------------------------------------

INSERT INTO tische (tisch_id, max_personen, aktiv, bereich, lage) VALUES
(1,2,true,'Tische 1-10','Frei'), (2,2,true,'Tische 1-10','Frei'),
(3,2,true,'Tische 1-10','Frei'), (4,2,true,'Tische 1-10','Frei'),
(5,2,true,'Tische 1-10','Frei'), (6,2,true,'Tische 1-10','Frei'),
(7,2,true,'Tische 1-10','Frei'), (8,2,true,'Tische 1-10','Frei'),
(9,2,true,'Tische 1-10','Frei'), (10,2,true,'Tische 1-10','Frei'),
(11,4,true,'Tische 11-20','Frei'), (12,4,true,'Tische 11-20','Frei'),
(13,4,true,'Tische 11-20','Frei'), (14,4,true,'Tische 11-20','Frei'),
(15,4,true,'Tische 11-20','Frei'), (16,4,true,'Tische 11-20','Frei'),
(17,4,true,'Tische 11-20','Frei'), (18,4,true,'Tische 11-20','Frei'),
(19,4,true,'Tische 11-20','Frei'), (20,4,true,'Tische 11-20','Frei'),
(21,6,true,'Tische 21-30','Frei'), (22,6,true,'Tische 21-30','Frei'),
(23,6,true,'Tische 21-30','Frei'), (24,6,true,'Tische 21-30','Frei'),
(25,6,true,'Tische 21-30','Frei'), (26,6,true,'Tische 21-30','Frei'),
(27,6,true,'Tische 21-30','Frei'), (28,6,true,'Tische 21-30','Frei'),
(29,6,true,'Tische 21-30','Frei'), (30,6,true,'Tische 21-30','Frei'),
(31,8,true,'Tische 31-40','Frei'), (32,8,true,'Tische 31-40','Frei'),
(33,8,true,'Tische 31-40','Frei'), (34,8,true,'Tische 31-40','Frei'),
(35,8,true,'Tische 31-40','Frei'), (36,10,true,'Tische 31-40','Frei'),
(37,10,true,'Tische 31-40','Frei'), (38,10,true,'Tische 31-40','Frei'),
(39,10,true,'Tische 31-40','Frei'), (40,10,true,'Tische 31-40','Frei');

INSERT INTO speisen (speisename, speisentyp, preis, zutaten, aktiv) VALUES
('Pizza Margherita','🍕 PIZZA', 8.50, 'Tomatensauce, Mozzarella',true),
('Pizza Salami','🍕 PIZZA', 9.50, 'Tomatensauce, Mozzarella, Salami',true),
('Pizza Prosciutto','🍕 PIZZA', 10.00, 'Tomatensauce, Mozzarella, Schinken',true),
('Pizza Funghi','🍕 PIZZA', 9.00, 'Tomatensauce, Mozzarella, Champignons',true),
('Pizza Hawaii','🍕 PIZZA', 10.50, 'Schinken, Ananas, Käse',true),
('Pizza Tonno','🍕 PIZZA', 11.00, 'Thunfisch, Zwiebeln, Käse',true),
('Pizza Quattro Formaggi','🍕 PIZZA', 11.50, '4 Käsesorten',true),
('Pizza Vegetaria','🍕 PIZZA', 10.00, 'Gemüse, Käse',true),
('Pasta Bolognese','🍝 PASTA', 11.50, 'Rinderhack, Tomatensauce',true),
('Pasta Carbonara','🍝 PASTA', 12.00, 'Sahnesauce, Ei, Speck',true),
('Pasta Napoli','🍝 PASTA', 9.50, 'Tomatensauce',true),
('Pasta Alfredo','🍝 PASTA', 12.50, 'Sahnesauce, Hähnchen',true),
('Insalata Mista','🥗 SALATE', 6.50, 'Salat, Tomaten, Gurken',true),
('Caesar Salad','🥗 SALATE', 9.00, 'Hähnchen, Parmesan, Croutons',true),
('Cola 0,33l','🥤 GETRÄNKE', 3.00, '',true),
('Cola Zero 0,33l','🥤 GETRÄNKE', 3.00, '',true),
('Fanta 0,33l','🥤 GETRÄNKE', 3.00, '',true),
('Sprite 0,33l','🥤 GETRÄNKE', 3.00, '',true),
('Mineralwasser 0,5l','🥤 GETRÄNKE', 2.50, '',true),
('Apfelschorle 0,5l','🥤 GETRÄNKE', 3.00, '',true),
('Tiramisu','🍰 DESSERT', 5.00, 'Mascarpone, Kaffee',true),
('Panna Cotta','🍰 DESSERT', 4.50, 'Sahne, Vanille',true),
('Schokoladenkuchen','🍰 DESSERT', 4.00, 'Schokolade',true);

INSERT INTO mitarbeiter (vorname, nachname, bereich, passwort, rolle, aktiv) VALUES
('Luigi','Rossi','Management','manager1','management',true),
('Marco','Habibi','Tische 1-10','service1','service',true),
('Luca','Romano','Tische 11-20','service2','service',true),
('Elena','Ferrari','Tische 21-30','service3','service',true),
('Maximilian','Achmed','Tische 31-40','service4','service',true),
('Mario','Test','Küche','koch1','koch',true),
('Giulia','Bianchi','Küche','koch2','koch',true),
('Sara','Conti','Kasse','kasse1','kasse',true),
('Lucas','Huber','EDV','admin1','admin',true),
('Diaa','Admin','EDV','admin2','admin',true),
('Julian','Ligenza','EDV','admin3','admin',true);

-- ------------------------------------------------------------
-- VIEWS UND ANALYSEN
-- ------------------------------------------------------------

DROP VIEW IF EXISTS UmsatzProTag;
CREATE VIEW UmsatzProTag AS
SELECT DATE(b.datum) AS tag, SUM(p.menge * p.preis_beim_kauf) AS umsatz
FROM bestellungen b JOIN bestellposition p ON b.bestellnr = p.bestellnr_fk
GROUP BY DATE(b.datum);

DROP VIEW IF EXISTS UmsatzProWoche;
CREATE VIEW UmsatzProWoche AS
SELECT YEAR(b.datum) AS jahr, WEEK(b.datum, 1) AS kalenderwoche, SUM(p.menge * p.preis_beim_kauf) AS umsatz
FROM bestellungen b JOIN bestellposition p ON b.bestellnr = p.bestellnr_fk
GROUP BY jahr, kalenderwoche;

DROP VIEW IF EXISTS UmsatzProMonat;
CREATE VIEW UmsatzProMonat AS
SELECT YEAR(b.datum) AS jahr, MONTH(b.datum) AS monat, SUM(p.menge * p.preis_beim_kauf) AS umsatz
FROM bestellungen b JOIN bestellposition p ON b.bestellnr = p.bestellnr_fk
GROUP BY jahr, monat;

DROP VIEW IF EXISTS UmsatzProGast;
CREATE VIEW UmsatzProGast AS
SELECT g.gastid, CONCAT(g.gastvorname, ' ', g.gastnachname) AS gastname, SUM(p.menge * p.preis_beim_kauf) AS umsatz
FROM gast g JOIN bestellungen b ON g.gastid = b.gast_id_fk
JOIN bestellposition p ON b.bestellnr = p.bestellnr_fk
GROUP BY g.gastid, gastname
ORDER BY umsatz DESC;

-- Tisch-Lagen anpassen
UPDATE tische SET ort = 'Eingang' WHERE tisch_id BETWEEN 1 AND 5;
UPDATE tische SET ort = 'Fenster' WHERE tisch_id BETWEEN 6 AND 10;
UPDATE tische SET ort = 'Vorne Links' WHERE tisch_id BETWEEN 11 AND 15;
UPDATE tische SET ort = 'Vorne Rechts' WHERE tisch_id BETWEEN 16 AND 20;
UPDATE tische SET ort = 'Neben WC' WHERE tisch_id BETWEEN 21 AND 25;
UPDATE tische SET ort = 'Mitte' WHERE tisch_id BETWEEN 26 AND 30;
UPDATE tische SET ort = 'Terrasse' WHERE tisch_id BETWEEN 31 AND 35;
UPDATE tische SET ort = 'Hinten' WHERE tisch_id BETWEEN 36 AND 40;

-- ------------------------------------------------------------
-- KONTROLL-SELECTS (ZUM TESTEN)
-- ------------------------------------------------------------
SELECT * FROM speisen;
SELECT * FROM mitarbeiter;
SELECT * FROM tische;
SELECT * FROM gast;
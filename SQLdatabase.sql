drop database pizzaprojekt;
Create database pizzaprojekt;
use pizzaprojekt;

Create table benutzer(
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



create table reservierungen(
    gastname varchar(100),
    tisch_fk int,
    slot_fk int,
    datum_fk datetime,
    personenanzahl int,
    primary key(gastname,datum_fk,slot_fk),

    -- Ein Fremdschlüssel, der auf alle drei Spalten gleichzeitig verweist
    foreign key(tisch_fk, slot_fk, datum_fk) references tische (tisch, slot, datum)
);

create table rechnungen(
rechnungsnr int not null unique primary key,
bestellnr_fk int,
gesamtpreis double,

foreign key(bestellnr_fk) references Bestellungen (bestellnr)
);


-- inserts

INSERT INTO speisen (speisename, preis, zutaten) VALUES
-- 🍕 PIZZA
('Pizza Margherita', 8.50, 'Tomatensauce, Mozzarella'),
('Pizza Salami', 9.50, 'Tomatensauce, Mozzarella, Salami'),
('Pizza Prosciutto', 10.00, 'Tomatensauce, Mozzarella, Schinken'),
('Pizza Funghi', 9.00, 'Tomatensauce, Mozzarella, Champignons'),
('Pizza Hawaii', 10.50, 'Schinken, Ananas, Käse'),
('Pizza Tonno', 11.00, 'Thunfisch, Zwiebeln, Käse'),
('Pizza Quattro Formaggi', 11.50, '4 Käsesorten'),
('Pizza Vegetaria', 10.00, 'Gemüse, Käse'),

-- 🍝 PASTA
('Pasta Bolognese', 11.50, 'Rinderhack, Tomatensauce'),
('Pasta Carbonara', 12.00, 'Sahnesauce, Ei, Speck'),
('Pasta Napoli', 9.50, 'Tomatensauce'),
('Pasta Alfredo', 12.50, 'Sahnesauce, Hähnchen'),

-- 🥗 SALATE
('Insalata Mista', 6.50, 'Salat, Tomaten, Gurken'),
('Caesar Salad', 9.00, 'Hähnchen, Parmesan, Croutons'),

-- 🥤 GETRÄNKE
('Cola 0,33l', 3.00, 'Getränk'),
('Cola Zero 0,33l', 3.00, 'Getränk'),
('Fanta 0,33l', 3.00, 'Getränk'),
('Sprite 0,33l', 3.00, 'Getränk'),
('Mineralwasser 0,5l', 2.50, 'Getränk'),
('Apfelschorle 0,5l', 3.00, 'Getränk'),

-- 🍰 DESSERT
('Tiramisu', 5.00, 'Mascarpone, Kaffee'),
('Panna Cotta', 4.50, 'Sahne, Vanille'),
('Schokoladenkuchen', 4.00, 'Schokolade');

INSERT INTO mitarbeiter (personalnr, name, berreich) VALUES
(1, 'Marco Rossi', 'Service'),
(2, 'Giulia Bianchi', 'Küche'),
(3, 'Luca Romano', 'Service'),
(4, 'Sara Conti', 'Kasse'),
(5, 'Antonio Greco', 'Küche'),
(6, 'Elena Ferrari', 'Service'),
(7,'Lucas Huber','EDV Admin'),
(8,'diaa','EDV Admin'),
(9,'Julian','EDV Admin');

-- berechnungen
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



insert into benutzer(username,passwort) value
('admin','admin');

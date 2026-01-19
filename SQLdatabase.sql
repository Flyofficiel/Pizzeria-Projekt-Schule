drop database pizzaprojekt;
Create database pizzaprojekt;
use pizzaprojekt;

-- Tabellen

create table speisen(
speiseid int unique primary key,
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
    preis_fk DOUBLE,

    FOREIGN KEY (bestellnr_fk) REFERENCES bestellungen(bestellnr),
    FOREIGN KEY (speisename_fk,preis_fk) REFERENCES speisen(speisename,preis)
);



create table reservierungen(
    gastname varchar(100),
    tisch_fk int,
    slot_fk int,
    datum_fk datetime,
    personenanzahl int,
    telephonnunmmr int,
    primary key(telephonnunmmr,datum_fk,slot_fk),

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

INSERT INTO speisen (speiseid,speisename, preis, zutaten) VALUES
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
(12,'Insalata Mista', 6.50, 'Salat, Tomaten, Gurken'),
(13,'Caesar Salad', 9.00, 'Hähnchen, Parmesan, Croutons'),

-- 🥤 GETRÄNKE
(14,'Cola 0,33l', 3.00, ''),
(15,'Cola Zero 0,33l', 3.00, ''),
(16,'Fanta 0,33l', 3.00, ''),
(17,'Sprite 0,33l', 3.00, ''),
(18,'Mineralwasser 0,5l', 2.50, ''),
(19,'Apfelschorle 0,5l', 3.00, ''),

-- 🍰 DESSERT
(20,'Tiramisu', 5.00, 'Mascarpone, Kaffee'),
(21,'Panna Cotta', 4.50, 'Sahne, Vanille'),
(22,'Schokoladenkuchen', 4.00, 'Schokolade');

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

SELECT 
    SUM(menge * preis_fk) AS gesamtumsatz
FROM bestellposition;

-- Umsatz pro Tag

SELECT 
    DATE(b.datum) AS tag,
    SUM(p.menge * p.preis_fk) AS umsatz
FROM bestellungen b
JOIN bestellposition p 
    ON b.bestellnr = p.bestellnr_fk
GROUP BY DATE(b.datum);

-- Umsatz pro Mitarbeiter

SELECT 
    m.name,
    SUM(p.menge * p.preis_fk) AS umsatz
FROM mitarbeiter m
JOIN bestellungen b 
    ON m.personalnr = b.personalnr_fk
JOIN bestellposition p 
    ON b.bestellnr = p.bestellnr_fk
GROUP BY m.name;

-- Umsatz pro Tisch

SELECT 
    b.tisch_fk,
    SUM(p.menge * p.preis_fk) AS umsatz
FROM bestellungen b

JOIN bestellposition p 
    ON b.bestellnr = p.bestellnr_fk
GROUP BY b.tisch_fk;

-- Beliebteste Speisen

SELECT 
    s.speisename,
    SUM(p.menge) AS verkauft
FROM speisen s
JOIN bestellposition p
    ON s.speisename = p.speisename_fk
GROUP BY s.speisename
ORDER BY verkauft DESC;

--

CREATE VIEW UmsatzProTag AS
SELECT 
    DATE(b.datum) AS tag,
    SUM(p.menge * p.preis_fk) AS umsatz
FROM bestellungen b
JOIN bestellposition p 
ON b.bestellnr = p.bestellnr_fk
GROUP BY DATE(b.datum);


select * from speisen;

select * from mitarbeiter
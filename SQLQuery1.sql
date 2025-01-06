CREATE TABLE Fahrzeuge (
    FahrzeugId INT PRIMARY KEY IDENTITY,
    Marke NVARCHAR(50) NOT NULL,
    Modell NVARCHAR(50) NOT NULL,
    Fahrzeugtyp NVARCHAR(50) NOT NULL,
    Baujahr INT NOT NULL,
    Preis DECIMAL(10, 2) NOT NULL,
    Verfügbarkeit NVARCHAR(20) NOT NULL,
    BildUrl NVARCHAR(200)
);

INSERT INTO Fahrzeuge (Marke, Modell, Fahrzeugtyp, Baujahr, Preis, Verfügbarkeit, BildUrl)
VALUES 
('BMW', 'X5', 'SUV', 2021, 45000.00, 'Verfügbar', '/images/bmw-x5.jpg'),
('Audi', 'A4', 'Limousine', 2020, 35000.00, 'Nicht verfügbar', '/images/audi-a4.jpg'),
('Tesla', 'Model 3', 'Limousine', 2023, 55000.00, 'Verfügbar', '/images/tesla-model3.jpg');

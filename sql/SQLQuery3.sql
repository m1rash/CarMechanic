create table Cars(
	carID int identity(1,1) primary key,
	carType varchar(50) not null,
	carModel varchar(50) not null
);
insert into Cars (carType, carModel) values
('Легковая','Toyota Supra'),
('Легковая','Mitsubishi Lancer'),
('Легковая','Tesla Model 3'),
('Легковая','Tesla Model X'),
('Грузовая','УАЗ 2360');
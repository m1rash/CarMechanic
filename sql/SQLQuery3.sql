create table Cars(
	carID int identity(1,1) primary key,
	carType varchar(50) not null,
	carModel varchar(50) not null
);
insert into Cars (carType, carModel) values
('��������','Toyota Supra'),
('��������','Mitsubishi Lancer'),
('��������','Tesla Model 3'),
('��������','Tesla Model X'),
('��������','��� 2360');
Create table Roles(
	roleID int identity(1,1) primary key,
	roleName VARCHAR(50) not null
);
insert into Roles (roleName) values
('Менеджер'),
('Автомеханик'),
('Оператор'),
('Заказчик');
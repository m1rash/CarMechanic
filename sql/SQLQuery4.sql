create table Statuses(
	statusID int identity(1,1) primary key,
	statusName varchar (50) not null
);
insert into Statuses (statusName) values
('Новая заявка'),
('В процессе ремонта'),
('Готов к выдаче');
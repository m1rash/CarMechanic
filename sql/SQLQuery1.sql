Create table Users(
	userID int identity(1,1) PRIMARY KEY,
	fio VARCHAR(100),
	phone VARCHAR(20),
	login varchar(50) unique not null,
	password varchar(50) not null,
	roleID int,
	foreign key (roleID) references Roles(roleID)
);
insert into Users (fio, phone, login, password, roleID) values
('�������� ���� ���������','89114761098','login1','pass1',1),
('����� ������ ��������','89116588913','login2','pass2',2),
('������ �������� ������������','89113450908','login3','pass3',2),
('���������� ������ ���������','89116009141','login4','pass4',3),
('����� ���� ��������','89113098701','login5','pass5',3),
('������ ���� ��������','89114670981','login11','pass11',4),
('������ ���� ��������','89116729812','login12','pass12',4),
('������� ������ ���������','89110987654','login13','pass13',2);
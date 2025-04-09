use MobileDb
go
INSERT INTO categories
VALUES 
('nexus',1,'/images/category/image/ccdb324d-8f1b-4eb5-8818-f09a3feb9a19.png','/images/category/logo/75e31301-7413-432e-98b5-b8f6de4bcbd0.png',GETDATE()),
('iphone',1,'/images/category/image/5b3b541f-5a84-4fda-96ac-b9b61698d3ca.png','/images/category/logo/3610fc88-98bb-4263-b4ab-64a6555a4423.png',GETDATE()),
('samsung',1,'/images/category/image/8aab0e8b-32e1-4426-a3aa-d8164dbcfa36.png','/images/category/logo/3fdb236f-dbaa-4ad4-9256-33c1972c5bec.png',GETDATE()),
('htc',1,'/images/category/image/7e2f3f8f-9da8-4ceb-a963-a6b1c2aa1270.png','/images/category/logo/340f29a3-1b90-4d92-ac80-f4f579f95cba.png',GETDATE()),
('alcatel',1,'/images/category/image/f5d56d4c-d420-4421-bd96-decc8162815b.png','/images/category/logo/afb7dd1e-f57b-458e-b412-00008fd5548b.png',GETDATE()),
('pixel',1,'/images/category/image/2153211c-eb13-4e3e-8a4f-b81f9d5dc830.png','/images/category/logo/7f075a7d-70c2-42ee-ad8f-cdaefc11f58a.png',GETDATE()),
('vivo',1,'/images/category/image/3658d828-31a0-4f4c-bc20-3aa507f40374.png','/images/category/logo/61c5f58b-22d0-4081-97ff-530fffc5c69a.png',GETDATE())
go
INSERT INTO colors
VALUES ('Trắng'),('Đen'),('Xanh'),('Tím'),('Hồng')
go
INSERT INTO products
VALUES 
('P001','Iphone xs max',5000000,2,1,1,'6.5 inch OLED','7 MP','12 MP + 12 MP','Apple A12 Bionic','IOS 12','Wi-Fi, Bluetooth 5.0, NFC','512GB',GETDATE()),
('P002','Samsung Galaxy S24',17990000,3,1,1,'6.8 inch Dynamic AMOLED','12 MP','108 MP + 12 MP + 10 MP','Snapdragon 8 Gen 3','Android 14','Wi-Fi 6E, Bluetooth 5.3, NFC, 5G','512GB',GETDATE()),
('P003','Nexus 6P',9900000,1,1,1,'5.7 inch AMOLED','8 MP','12.3 MP','Qualcomm Snapdragon 810','Android 6.0 Marshmallow','Wi-Fi, Bluetooth 4.2, NFC','128GB',GETDATE()),
('P004','HTC U23 Pro 5G',13000000,4,1,1,'6.7 inch OLED, 120Hz','32 MP','108 MP + 8 MP + 5 MP + 2 MP','Qualcomm Snapdragon 7 Gen 1','Android 13','Wi-Fi 6, Bluetooth 5.2, NFC, 5G','256GB',GETDATE()),
('P005','IPhone 15 Pro',28990000,2,1,1,'6.1 inch Super Retina XDR, ProMotion 120Hz','12 MP','48 MP + 12 MP + 12 MP (Telephoto)','Apple A17 Pro','iOS 17','Wi-Fi 6E, Bluetooth 5.3, 5G, USB-C','1TB',GETDATE())
go
INSERT INTO productsImages
VALUES 
('P001','/images/product/thumb/3af0788e-876f-46eb-831d-67c5a2f2a22d.jpg','/images/product/images/264ab580-6904-447f-ac56-81061fd5275f.jpg;/images/product/images/448455c4-f6fa-49dd-a977-010f8b77889b.jpg;/images/product/images/be73c63f-eacf-40a8-9d90-eb24981883ce.jpg'),
('P002','/images/product/thumb/02a369d2-813c-460f-9f05-58584956fcd1.jpg','/images/product/images/9e138129-62fe-4ebd-ac78-c79a1bfe9e1b.jpg;/images/product/images/143e00c7-6632-4e1f-b8cb-0a2213a876fe.jpg;/images/product/images/e6cf864c-5426-4ab9-b795-0f32362b0960.jpg'),
('P003','/images/product/thumb/d7e0d56a-fdab-4070-a039-a6da6a6fd472.jpg','/images/product/images/7bcafe73-9649-447f-aa4f-c1dccbfa784b.jpg;/images/product/images/ae1df470-a3b0-4b26-85d0-7d7d3d4da3ae.jpg;/images/product/images/0fc195b0-1ab1-41e1-ac11-2a37a70095a1.jpg'),
('P004','/images/product/thumb/7253ef6b-3e02-4190-b5ff-9c1ebe42a5dc.jpg','/images/product/images/8bffef5c-bf25-4320-ae3e-646c90dc4404.jpg;/images/product/images/25e7608f-d208-488d-be0b-d14f65e81c3c.jpg;/images/product/images/59681c9c-0698-4081-b460-848003478f7b.jpg'),
('P005','/images/product/thumb/a83f6d3a-5858-4346-b0bb-6a779b022c48.jpg','/images/product/images/8d23f8b4-c361-436b-9896-9cb76693196d.jpg;/images/product/images/45cf36ab-28c0-4219-ac6c-11c0f0e7f81c.jpg;/images/product/images/867948a0-2e0b-4e59-8c27-e6209e2a78b2.jpg')
go
INSERT INTO productsColors
VALUES 
('P001','Đen, Xanh'),
('P002','Đen, Xanh'),
('P003','Đen, Xanh'),
('P004','Đen, Xanh'),
('P005','Đen, Xanh')
go
INSERT INTO users
VALUES
('Guest','','','','','',0,GETDATE(),'',''),
('admin','admin','admin@gmail.com','0123456789','e10adc3949ba59abbe56e057f20f883e','Ha Noi',1,GETDATE(),'',''),
('user01','Nguyen Quoc Viet','vietmoc1702@gmail.com','0963255780','e10adc3949ba59abbe56e057f20f883e','Ha Noi',0,GETDATE(),'','')

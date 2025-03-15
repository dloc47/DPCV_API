CREATE DATABASE  IF NOT EXISTS `dpcv_db_updated` /*!40100 DEFAULT CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci */ /*!80016 DEFAULT ENCRYPTION='N' */;
USE `dpcv_db_updated`;
-- MySQL dump 10.13  Distrib 8.0.41, for Win64 (x86_64)
--
-- Host: localhost    Database: dpcv_db_updated
-- ------------------------------------------------------
-- Server version	8.0.40

/*!40101 SET @OLD_CHARACTER_SET_CLIENT=@@CHARACTER_SET_CLIENT */;
/*!40101 SET @OLD_CHARACTER_SET_RESULTS=@@CHARACTER_SET_RESULTS */;
/*!40101 SET @OLD_COLLATION_CONNECTION=@@COLLATION_CONNECTION */;
/*!50503 SET NAMES utf8 */;
/*!40103 SET @OLD_TIME_ZONE=@@TIME_ZONE */;
/*!40103 SET TIME_ZONE='+00:00' */;
/*!40014 SET @OLD_UNIQUE_CHECKS=@@UNIQUE_CHECKS, UNIQUE_CHECKS=0 */;
/*!40014 SET @OLD_FOREIGN_KEY_CHECKS=@@FOREIGN_KEY_CHECKS, FOREIGN_KEY_CHECKS=0 */;
/*!40101 SET @OLD_SQL_MODE=@@SQL_MODE, SQL_MODE='NO_AUTO_VALUE_ON_ZERO' */;
/*!40111 SET @OLD_SQL_NOTES=@@SQL_NOTES, SQL_NOTES=0 */;

--
-- Table structure for table `activities`
--

DROP TABLE IF EXISTS `activities`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `activities` (
  `activity_id` int NOT NULL AUTO_INCREMENT,
  `activity_name` varchar(255) NOT NULL,
  `description` text,
  `tags` json DEFAULT NULL,
  `committee_id` int NOT NULL,
  `homestay_id` int DEFAULT NULL,
  `isVerifiable` tinyint DEFAULT '0',
  `verification_status_id` int DEFAULT NULL,
  `is_active` tinyint(1) NOT NULL DEFAULT '1',
  PRIMARY KEY (`activity_id`),
  KEY `committee_id` (`committee_id`),
  KEY `homestay_id` (`homestay_id`),
  KEY `verification_status_id` (`verification_status_id`),
  CONSTRAINT `activities_ibfk_1` FOREIGN KEY (`committee_id`) REFERENCES `committees` (`committee_id`) ON DELETE CASCADE,
  CONSTRAINT `activities_ibfk_2` FOREIGN KEY (`homestay_id`) REFERENCES `homestays` (`homestay_id`) ON DELETE SET NULL
) ENGINE=InnoDB AUTO_INCREMENT=18 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `activities`
--

LOCK TABLES `activities` WRITE;
/*!40000 ALTER TABLE `activities` DISABLE KEYS */;
INSERT INTO `activities` VALUES (1,'Trekking','Trekking in the beautiful hills of East Sikkim.','[\"adventure\", \"outdoor\", \"fun\"]',1,NULL,1,1,1),(2,'Cultural Show','A cultural show showcasing local traditions in West Sikkim.','[\"cultural\", \"heritage\", \"historical\"]',2,NULL,0,2,1),(4,'Test Activity','Lorem Ipsum lassan','[\"relaxing\", \"wellness\", \"spa\"]',2,NULL,1,2,1),(13,'Fixed Response body2','string','[\"trekking\", \"nature\", \"wildlife\"]',1,NULL,1,2,1),(14,'Fixed Response body3','string','[\"water sports\", \"beach\", \"snorkeling\"]',1,NULL,1,2,1),(15,'Fixed Response body4','string','[\"food tasting\", \"local cuisine\", \"street food\"]',1,NULL,1,2,1),(16,'Fixed Response body5','string','[\"music\", \"festival\", \"live performance\"]',1,NULL,1,2,1);
/*!40000 ALTER TABLE `activities` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `committees`
--

DROP TABLE IF EXISTS `committees`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `committees` (
  `committee_id` int NOT NULL AUTO_INCREMENT,
  `committee_name` varchar(255) NOT NULL,
  `description` text,
  `district_id` int NOT NULL,
  `contact_number` varchar(20) DEFAULT NULL,
  `email` varchar(191) DEFAULT NULL,
  `address` text,
  `tags` json DEFAULT NULL,
  `tourist_attractions` json DEFAULT NULL,
  `latitude` decimal(10,6) DEFAULT NULL,
  `longitude` decimal(10,6) DEFAULT NULL,
  `leadership` json DEFAULT NULL,
  `isVerifiable` tinyint DEFAULT '0',
  `verification_status_id` int DEFAULT NULL,
  `is_active` tinyint(1) NOT NULL DEFAULT '1',
  PRIMARY KEY (`committee_id`),
  KEY `district_id` (`district_id`),
  KEY `verification_status_id` (`verification_status_id`),
  CONSTRAINT `committees_ibfk_1` FOREIGN KEY (`district_id`) REFERENCES `districts` (`district_id`) ON DELETE CASCADE
) ENGINE=InnoDB AUTO_INCREMENT=6 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `committees`
--

LOCK TABLES `committees` WRITE;
/*!40000 ALTER TABLE `committees` DISABLE KEYS */;
INSERT INTO `committees` VALUES (1,'East Sikkim Committee','Martam is a picturesque village nestled in the Eastern part of Sikkim, India. Surrounded by lush forests and terraced paddy fields, it offers a serene retreat for nature enthusiasts.',1,'9876543210','east@committee.com','East Sikkim, India','[\"Tourism\", \"Eco\"]','[{\"name\": \"Fambong La Wildlife Sanctuary\", \"description\": \"Located nearby, this sanctuary is home to a variety of wildlife species and offers trekking opportunities.\"}, {\"name\": \"Rumtek Monastery\", \"description\": \"A significant Buddhist monastery known for its architecture and spiritual ambiance.\"}, {\"name\": \"Trekking Trails\", \"description\": \"The region offers several trekking routes with panoramic views of the Himalayas, including the Kanchenjunga range.\"}]',27.318900,88.622100,'[{\"bio\": \"Serving for over a decade in community development, implementing sustainable tourism practices.\", \"name\": \"Tenzing Sherpa\", \"role\": \"President\"}, {\"bio\": \"Dedicated educator and cultural ambassador, organizing cultural exchange programs and educational workshops.\", \"name\": \"Lhamo Dolma\", \"role\": \"Secretary\"}]',1,1,1),(2,'West Sikkim Committee','Yuksom is a historic town in West Sikkim, known as the first capital of Sikkim. It is the gateway to the famous Dzongri and Goechala treks and holds great cultural and religious significance.',2,'9876543211','west@committee.com','West Sikkim, India','[\"Adventure\", \"Cultural\"]','[{\"name\": \"Fambong La Wildlife Sanctuary\", \"description\": \"Located nearby, this sanctuary is home to a variety of wildlife species and offers trekking opportunities.\"}, {\"name\": \"Rumtek Monastery\", \"description\": \"A significant Buddhist monastery known for its architecture and spiritual ambiance.\"}, {\"name\": \"Trekking Trails\", \"description\": \"The region offers several trekking routes with panoramic views of the Himalayas, including the Kanchenjunga range.\"}]',27.318900,88.622100,'[{\"bio\": \"Serving for over a decade in community development, implementing sustainable tourism practices.\", \"name\": \"Tenzing Sherpa\", \"role\": \"President\"}, {\"bio\": \"Dedicated educator and cultural ambassador, organizing cultural exchange programs and educational workshops.\", \"name\": \"Lhamo Dolma\", \"role\": \"Secretary\"}]',1,2,1),(3,'Martam Village Committee','Martam is a picturesque village nestled in the Eastern part of Sikkim, India. Surrounded by lush forests and terraced paddy fields, it offers a serene retreat for nature enthusiasts.',1,'+91 9876543210','martamcommittee@example.com','Martam, East Sikkim, India','[\"eco-tourism\", \"heritage\", \"trekking\"]','[{\"name\": \"Fambong La Wildlife Sanctuary\", \"description\": \"Located nearby, this sanctuary is home to a variety of wildlife species and offers trekking opportunities.\"}, {\"name\": \"Rumtek Monastery\", \"description\": \"A significant Buddhist monastery known for its architecture and spiritual ambiance.\"}, {\"name\": \"Trekking Trails\", \"description\": \"The region offers several trekking routes with panoramic views of the Himalayas, including the Kanchenjunga range.\"}]',27.318900,88.622100,'[{\"bio\": \"Serving for over a decade in community development, implementing sustainable tourism practices.\", \"name\": \"Tenzing Sherpa\", \"role\": \"President\"}, {\"bio\": \"Dedicated educator and cultural ambassador, organizing cultural exchange programs and educational workshops.\", \"name\": \"Lhamo Dolma\", \"role\": \"Secretary\"}]',1,1,1),(4,'North Sikkim Committee','A beautiful committee representing the diverse culture and tourism of North Sikkim.',2,'9123456789','north@committee.com','Lachen, North Sikkim, India','[\"Adventure\", \"Culture\", \"Eco\"]','[{\"name\": \"Gurudongmar Lake\", \"description\": \"A high-altitude lake offering breathtaking views and spiritual significance.\"}, {\"name\": \"Lachung Valley\", \"description\": \"A scenic valley known for its apple orchards, waterfalls, and snow-capped mountains.\"}]',27.700500,88.693600,'[{\"bio\": \"An experienced leader in rural tourism development.\", \"name\": \"Karma Bhutia\", \"role\": \"President\"}, {\"bio\": \"Cultural enthusiast promoting traditional arts and crafts.\", \"name\": \"Sonam Doma\", \"role\": \"Secretary\"}]',1,2,1),(5,'Green Valley Committee','A community-driven initiative to promote eco-tourism and sustainable living in Green Valley.',3,'+91 6294016508','greenvalley@committee.com','Green Valley, South Sikkim, India','[\"Eco-Tourism\", \"Sustainability\", \"Community\"]','[{\"Name\": \"Green Valley Eco Park\", \"Description\": \"A serene eco-park featuring lush greenery, organic farming, and guided nature walks.\"}, {\"Name\": \"Sunset Viewpoint\", \"Description\": \"A popular spot for breathtaking sunset views over the valley.\"}]',27.512300,88.765400,'[{\"Bio\": \"A dedicated environmentalist working towards sustainable tourism and rural development.\", \"Name\": \"AnkitGurung\", \"Role\": \"President\"}, {\"Bio\": \"A cultural heritage expert focused on preserving local traditions and crafts.\", \"Name\": \"Maya Tamang\", \"Role\": \"Secretary\"}]',1,2,1);
/*!40000 ALTER TABLE `committees` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `districts`
--

DROP TABLE IF EXISTS `districts`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `districts` (
  `district_id` int NOT NULL AUTO_INCREMENT,
  `district_name` varchar(191) NOT NULL,
  `region` enum('North','East','West','South') NOT NULL,
  PRIMARY KEY (`district_id`),
  UNIQUE KEY `district_name` (`district_name`)
) ENGINE=InnoDB AUTO_INCREMENT=7 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `districts`
--

LOCK TABLES `districts` WRITE;
/*!40000 ALTER TABLE `districts` DISABLE KEYS */;
INSERT INTO `districts` VALUES (1,'East Sikkim','East'),(2,'West Sikkim','West'),(3,'North Sikkim','North'),(4,'South Sikkim','South'),(5,'Gangtok','East'),(6,'Mangan','North');
/*!40000 ALTER TABLE `districts` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `events`
--

DROP TABLE IF EXISTS `events`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `events` (
  `event_id` int NOT NULL AUTO_INCREMENT,
  `event_name` varchar(255) NOT NULL,
  `description` text NOT NULL,
  `start_date` date NOT NULL,
  `end_date` date NOT NULL,
  `location` varchar(255) NOT NULL,
  `committee_id` int NOT NULL,
  `tags` json DEFAULT NULL,
  `isVerifiable` tinyint DEFAULT '0',
  `verification_status_id` int DEFAULT NULL,
  `is_active` tinyint(1) NOT NULL DEFAULT '1',
  PRIMARY KEY (`event_id`),
  KEY `committee_id` (`committee_id`),
  KEY `verification_status_id` (`verification_status_id`),
  CONSTRAINT `events_ibfk_1` FOREIGN KEY (`committee_id`) REFERENCES `committees` (`committee_id`) ON DELETE CASCADE
) ENGINE=InnoDB AUTO_INCREMENT=3 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `events`
--

LOCK TABLES `events` WRITE;
/*!40000 ALTER TABLE `events` DISABLE KEYS */;
INSERT INTO `events` VALUES (1,'East Sikkim Festival','Annual festival of East Sikkim showcasing cultural heritage.','2025-03-10','2025-03-15','East Sikkim',1,'[\"Festival\", \"Cultural\"]',1,1,1),(2,'West Sikkim Adventure','An adventure sports event held in West Sikkim.','2025-05-20','2025-05-25','West Sikkim',2,'[\"Adventure\", \"Sports\"]',0,2,1);
/*!40000 ALTER TABLE `events` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `homestays`
--

DROP TABLE IF EXISTS `homestays`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `homestays` (
  `homestay_id` int NOT NULL AUTO_INCREMENT,
  `homestay_name` varchar(255) NOT NULL,
  `committee_id` int NOT NULL,
  `address` text NOT NULL,
  `description` text,
  `owner_name` varchar(255) NOT NULL,
  `owner_mobile` varchar(20) NOT NULL,
  `total_rooms` int NOT NULL,
  `room_tariff` decimal(10,2) NOT NULL,
  `tags` json DEFAULT NULL,
  `amenities` json DEFAULT NULL,
  `payment_methods` text,
  `social_media_links` json DEFAULT NULL,
  `isVerifiable` tinyint DEFAULT '0',
  `verification_status_id` int DEFAULT NULL,
  `is_active` tinyint(1) NOT NULL DEFAULT '1',
  PRIMARY KEY (`homestay_id`),
  KEY `committee_id` (`committee_id`),
  KEY `verification_status_id` (`verification_status_id`),
  CONSTRAINT `homestays_ibfk_1` FOREIGN KEY (`committee_id`) REFERENCES `committees` (`committee_id`) ON DELETE CASCADE
) ENGINE=InnoDB AUTO_INCREMENT=3 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `homestays`
--

LOCK TABLES `homestays` WRITE;
/*!40000 ALTER TABLE `homestays` DISABLE KEYS */;
INSERT INTO `homestays` VALUES (1,'Mountain View Homestay',2,'East Sikkim, India','Nestled in the serene hills, Mountain View Homestay offers breathtaking views of lush green valleys. With cozy rooms, warm hospitality, and easy access to trekking trails, it\'s an ideal retreat for nature lovers.','John Doe','9876543201',5,1500.00,'[\"Mountain\", \"Eco-Friendly\"]','[\"Free WiFi\", \"Air Conditioning\", \"Swimming Pool\", \"Breakfast Included\", \"Parking Available\"]','Credit Card, Cash, UPI, PayPal','{\"twitter\": \"https://twitter.com/homestay1\", \"youtube\": \"https://youtube.com/channel/example1\", \"facebook\": \"https://facebook.com/homestay1\", \"instagram\": \"https://instagram.com/homestay1\"}',1,1,1),(2,'Riverfront Homestay',1,'West Sikkim, India','Located right by the riverside, Riverfront Homestay provides a peaceful escape with the soothing sound of flowing water. Guests can enjoy spacious rooms, homemade delicacies, and activities like fishing and kayaking.','Jane Smith','9876543202',8,2000.00,'[\"River\", \"Luxury\"]','[\"Kitchenette\", \"Pet Friendly\", \"Gym Access\", \"24/7 Security\", \"Airport Shuttle\"]','Debit Card, Net Banking, Cash, Google Pay','{\"twitter\": \"https://twitter.com/homestay2\", \"youtube\": \"https://youtube.com/channel/example2\", \"facebook\": \"https://facebook.com/homestay2\", \"instagram\": \"https://instagram.com/homestay2\"}',0,2,1);
/*!40000 ALTER TABLE `homestays` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `images`
--

DROP TABLE IF EXISTS `images`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `images` (
  `image_id` int NOT NULL AUTO_INCREMENT,
  `image_url` varchar(255) NOT NULL,
  `original_image_name` varchar(255) DEFAULT NULL,
  `image_name` varchar(255) DEFAULT NULL,
  `file_size` bigint DEFAULT NULL,
  `mime_type` varchar(100) DEFAULT NULL,
  `entity_type` enum('Admin','Committee','Homestay','Activity','Event','Product') NOT NULL,
  `entity_id` int NOT NULL,
  `committee_id` int DEFAULT NULL,
  `uploaded_by` int DEFAULT NULL,
  `is_profile_image` tinyint(1) DEFAULT '0',
  `status` enum('Active','Archived','Deleted') DEFAULT 'Active',
  `created_at` timestamp NULL DEFAULT CURRENT_TIMESTAMP,
  `updated_at` timestamp NULL DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
  PRIMARY KEY (`image_id`),
  KEY `entity_type` (`entity_type`),
  KEY `entity_id` (`entity_id`),
  KEY `committee_id` (`committee_id`),
  KEY `uploaded_by` (`uploaded_by`),
  CONSTRAINT `fk_images_committees` FOREIGN KEY (`committee_id`) REFERENCES `committees` (`committee_id`) ON DELETE CASCADE
) ENGINE=InnoDB AUTO_INCREMENT=13 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `images`
--

LOCK TABLES `images` WRITE;
/*!40000 ALTER TABLE `images` DISABLE KEYS */;
INSERT INTO `images` VALUES (7,'/Uploads/2062c73c-c466-436d-9bf3-4a43b9d67460.jpeg','WhatsApp Image 2025-03-06 at 12.21.07 PM.jpeg','2062c73c-c466-436d-9bf3-4a43b9d67460.jpeg',3792,'image/jpeg','Homestay',2,1,3,0,'Active','2025-03-12 11:04:34','2025-03-13 09:53:28'),(8,'/Uploads/4c7a3f93-72dd-4152-b307-9be76a255ed5.jpg','AI-image.jpg','4c7a3f93-72dd-4152-b307-9be76a255ed5.jpg',191737,'image/jpeg','Committee',1,1,3,0,'Active','2025-03-12 11:05:18','2025-03-13 09:52:34'),(11,'/Uploads/2bbe3711-c42f-4123-8497-8b5b274b15cb.jpg','ryuuki-r-cute-anime.jpg','2bbe3711-c42f-4123-8497-8b5b274b15cb.jpg',538132,'image/jpeg','Homestay',2,1,3,1,'Active','2025-03-13 08:50:14','2025-03-13 09:53:28'),(12,'/Uploads/4adcd262-e89e-44f1-9bcc-caa4965a6340.jpg','d7ccd6c5e5a2e3434e72d93a38b9772d.jpg','4adcd262-e89e-44f1-9bcc-caa4965a6340.jpg',79382,'image/jpeg','Product',2,2,3,0,'Active','2025-03-15 04:45:33','2025-03-15 04:45:33');
/*!40000 ALTER TABLE `images` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `master_verification_status`
--

DROP TABLE IF EXISTS `master_verification_status`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `master_verification_status` (
  `verification_status_id` int NOT NULL AUTO_INCREMENT,
  `status_type` enum('Verified by Committee','Verified by Admin','Rejected') NOT NULL,
  PRIMARY KEY (`verification_status_id`)
) ENGINE=InnoDB AUTO_INCREMENT=4 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `master_verification_status`
--

LOCK TABLES `master_verification_status` WRITE;
/*!40000 ALTER TABLE `master_verification_status` DISABLE KEYS */;
INSERT INTO `master_verification_status` VALUES (1,'Verified by Committee'),(2,'Verified by Admin'),(3,'Rejected');
/*!40000 ALTER TABLE `master_verification_status` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `products`
--

DROP TABLE IF EXISTS `products`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `products` (
  `product_id` int NOT NULL AUTO_INCREMENT,
  `product_name` varchar(255) NOT NULL,
  `description` text,
  `metric_unit` varchar(50) NOT NULL DEFAULT 'Per Unit',
  `metric_value` decimal(10,2) DEFAULT NULL,
  `price` decimal(10,2) NOT NULL,
  `committee_id` int NOT NULL,
  `homestay_id` int DEFAULT NULL,
  `tags` json DEFAULT NULL,
  `isVerifiable` tinyint DEFAULT '0',
  `verification_status_id` int DEFAULT NULL,
  `is_active` tinyint(1) NOT NULL DEFAULT '1',
  PRIMARY KEY (`product_id`),
  KEY `committee_id` (`committee_id`),
  KEY `homestay_id` (`homestay_id`),
  KEY `verification_status_id` (`verification_status_id`),
  CONSTRAINT `products_ibfk_1` FOREIGN KEY (`committee_id`) REFERENCES `committees` (`committee_id`) ON DELETE CASCADE,
  CONSTRAINT `products_ibfk_2` FOREIGN KEY (`homestay_id`) REFERENCES `homestays` (`homestay_id`) ON DELETE SET NULL
) ENGINE=InnoDB AUTO_INCREMENT=3 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `products`
--

LOCK TABLES `products` WRITE;
/*!40000 ALTER TABLE `products` DISABLE KEYS */;
INSERT INTO `products` VALUES (1,'Sikkim Organic Tea','Organic tea grown in East Sikkim.','Kilogram',0.50,500.00,1,NULL,'[\"Organic\", \"Tea\"]',1,1,1),(2,'Handmade Woolen Shawl','Woolen shawl handmade by local artisans of West Sikkim.','Piece',1.00,1500.00,2,NULL,'[\"Handmade\", \"Woolen\"]',0,2,1);
/*!40000 ALTER TABLE `products` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `refreshtokens`
--

DROP TABLE IF EXISTS `refreshtokens`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `refreshtokens` (
  `Id` int NOT NULL AUTO_INCREMENT,
  `UserId` int NOT NULL,
  `Token` varchar(255) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci NOT NULL,
  `ExpiresAt` datetime NOT NULL,
  `CreatedAt` datetime DEFAULT CURRENT_TIMESTAMP,
  `IsRevoked` tinyint(1) NOT NULL DEFAULT '0',
  `RevokedAt` datetime DEFAULT NULL,
  PRIMARY KEY (`Id`),
  UNIQUE KEY `Token` (`Token`),
  KEY `UserId` (`UserId`),
  CONSTRAINT `refreshtokens_ibfk_1` FOREIGN KEY (`UserId`) REFERENCES `users` (`user_id`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `refreshtokens`
--

LOCK TABLES `refreshtokens` WRITE;
/*!40000 ALTER TABLE `refreshtokens` DISABLE KEYS */;
/*!40000 ALTER TABLE `refreshtokens` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `user_roles`
--

DROP TABLE IF EXISTS `user_roles`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `user_roles` (
  `role_id` int NOT NULL AUTO_INCREMENT,
  `role_name` enum('Admin','Committee') NOT NULL,
  PRIMARY KEY (`role_id`)
) ENGINE=InnoDB AUTO_INCREMENT=3 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `user_roles`
--

LOCK TABLES `user_roles` WRITE;
/*!40000 ALTER TABLE `user_roles` DISABLE KEYS */;
INSERT INTO `user_roles` VALUES (1,'Admin'),(2,'Committee');
/*!40000 ALTER TABLE `user_roles` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `users`
--

DROP TABLE IF EXISTS `users`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `users` (
  `user_id` int NOT NULL AUTO_INCREMENT,
  `name` varchar(255) NOT NULL,
  `email` varchar(191) NOT NULL,
  `password` varchar(255) NOT NULL,
  `role_id` int NOT NULL,
  `district_id` int DEFAULT NULL,
  `committee_id` int DEFAULT NULL,
  `is_active` tinyint(1) NOT NULL DEFAULT '1',
  `created_at` datetime DEFAULT CURRENT_TIMESTAMP,
  `updated_at` datetime DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
  PRIMARY KEY (`user_id`),
  UNIQUE KEY `email` (`email`),
  KEY `role_id` (`role_id`),
  KEY `district_id` (`district_id`),
  KEY `fk_users_committees` (`committee_id`),
  CONSTRAINT `fk_users_committees` FOREIGN KEY (`committee_id`) REFERENCES `committees` (`committee_id`) ON DELETE SET NULL,
  CONSTRAINT `users_ibfk_1` FOREIGN KEY (`role_id`) REFERENCES `user_roles` (`role_id`) ON DELETE CASCADE,
  CONSTRAINT `users_ibfk_2` FOREIGN KEY (`district_id`) REFERENCES `districts` (`district_id`) ON DELETE SET NULL
) ENGINE=InnoDB AUTO_INCREMENT=8 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `users`
--

LOCK TABLES `users` WRITE;
/*!40000 ALTER TABLE `users` DISABLE KEYS */;
INSERT INTO `users` VALUES (1,'Admin','admin@example.com','admin123',1,2,NULL,1,'2025-02-18 15:48:06','2025-02-19 15:18:02'),(2,'Gangtok Committee','gangtokcommittee@example.com','committee123',2,NULL,NULL,1,'2025-02-18 15:48:06','2025-02-19 15:18:02'),(3,'Trial Admin','try@admin.com','240be518fabd2724ddb6f04eeb1da5967448d7e831c08c8fa822809f74c720a9',1,NULL,NULL,1,'2025-02-19 12:25:04','2025-02-19 15:18:02');
/*!40000 ALTER TABLE `users` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Dumping routines for database 'dpcv_db_updated'
--
/*!50003 DROP PROCEDURE IF EXISTS `ArchiveUser` */;
/*!50003 SET @saved_cs_client      = @@character_set_client */ ;
/*!50003 SET @saved_cs_results     = @@character_set_results */ ;
/*!50003 SET @saved_col_connection = @@collation_connection */ ;
/*!50003 SET character_set_client  = utf8mb4 */ ;
/*!50003 SET character_set_results = utf8mb4 */ ;
/*!50003 SET collation_connection  = utf8mb4_0900_ai_ci */ ;
/*!50003 SET @saved_sql_mode       = @@sql_mode */ ;
/*!50003 SET sql_mode              = '' */ ;
DELIMITER ;;
CREATE DEFINER=`root`@`localhost` PROCEDURE `ArchiveUser`(IN p_UserId INT)
BEGIN
    UPDATE Users SET is_active = 0 WHERE user_id = p_UserId;
END ;;
DELIMITER ;
/*!50003 SET sql_mode              = @saved_sql_mode */ ;
/*!50003 SET character_set_client  = @saved_cs_client */ ;
/*!50003 SET character_set_results = @saved_cs_results */ ;
/*!50003 SET collation_connection  = @saved_col_connection */ ;
/*!50003 DROP PROCEDURE IF EXISTS `CreateActivity` */;
/*!50003 SET @saved_cs_client      = @@character_set_client */ ;
/*!50003 SET @saved_cs_results     = @@character_set_results */ ;
/*!50003 SET @saved_col_connection = @@collation_connection */ ;
/*!50003 SET character_set_client  = utf8mb4 */ ;
/*!50003 SET character_set_results = utf8mb4 */ ;
/*!50003 SET collation_connection  = utf8mb4_0900_ai_ci */ ;
/*!50003 SET @saved_sql_mode       = @@sql_mode */ ;
/*!50003 SET sql_mode              = '' */ ;
DELIMITER ;;
CREATE DEFINER=`root`@`localhost` PROCEDURE `CreateActivity`(
    IN p_activity_name VARCHAR(255),
    IN p_description TEXT,
    IN p_tags JSON,
    IN p_committee_id INT,
    IN p_homestay_id INT,
    IN p_isVerifiable BOOLEAN,
    IN p_verification_status_id INT,
    IN p_is_active BOOLEAN
)
BEGIN
    INSERT INTO activities (
        activity_name, description, tags, committee_id, homestay_id, isVerifiable, verification_status_id, is_active
    ) VALUES (
        p_activity_name, 
        NULLIF(p_description, ''), -- ✅ Convert empty string to NULL
        NULLIF(p_tags, '[]'),      -- ✅ Convert empty JSON array to NULL
        p_committee_id, 
        NULLIF(p_homestay_id, 0),  -- ✅ Convert 0 to NULL
        p_isVerifiable, 
        p_verification_status_id, 
        p_is_active
    );

    -- ✅ Return the newly inserted Activity ID
    SELECT LAST_INSERT_ID() AS activity_id;
END ;;
DELIMITER ;
/*!50003 SET sql_mode              = @saved_sql_mode */ ;
/*!50003 SET character_set_client  = @saved_cs_client */ ;
/*!50003 SET character_set_results = @saved_cs_results */ ;
/*!50003 SET collation_connection  = @saved_col_connection */ ;
/*!50003 DROP PROCEDURE IF EXISTS `CreateCommittee` */;
/*!50003 SET @saved_cs_client      = @@character_set_client */ ;
/*!50003 SET @saved_cs_results     = @@character_set_results */ ;
/*!50003 SET @saved_col_connection = @@collation_connection */ ;
/*!50003 SET character_set_client  = utf8mb4 */ ;
/*!50003 SET character_set_results = utf8mb4 */ ;
/*!50003 SET collation_connection  = utf8mb4_0900_ai_ci */ ;
/*!50003 SET @saved_sql_mode       = @@sql_mode */ ;
/*!50003 SET sql_mode              = '' */ ;
DELIMITER ;;
CREATE DEFINER=`root`@`localhost` PROCEDURE `CreateCommittee`(
    IN p_committee_name VARCHAR(255),
    IN p_description TEXT,
    IN p_district_id INT,
    IN p_contact_number VARCHAR(20),
    IN p_email VARCHAR(191),
    IN p_address TEXT,
    IN p_tags JSON,
    IN p_tourist_attractions JSON,
    IN p_latitude DECIMAL(10,6),
    IN p_longitude DECIMAL(10,6),
    IN p_leadership JSON,
    IN p_isVerifiable TINYINT,
    IN p_verification_status_id INT,
    IN p_is_active TINYINT
)
BEGIN
    INSERT INTO committees (
        committee_name, description, district_id, 
        contact_number, email, address, tags, 
        tourist_attractions, latitude, longitude, 
        leadership, isVerifiable, verification_status_id, is_active
    ) 
    VALUES (
        p_committee_name, p_description, p_district_id, 
        p_contact_number, p_email, p_address, p_tags, 
        p_tourist_attractions, p_latitude, p_longitude, 
        p_leadership, p_isVerifiable, p_verification_status_id, p_is_active
    );
END ;;
DELIMITER ;
/*!50003 SET sql_mode              = @saved_sql_mode */ ;
/*!50003 SET character_set_client  = @saved_cs_client */ ;
/*!50003 SET character_set_results = @saved_cs_results */ ;
/*!50003 SET collation_connection  = @saved_col_connection */ ;
/*!50003 DROP PROCEDURE IF EXISTS `CreateDistrict` */;
/*!50003 SET @saved_cs_client      = @@character_set_client */ ;
/*!50003 SET @saved_cs_results     = @@character_set_results */ ;
/*!50003 SET @saved_col_connection = @@collation_connection */ ;
/*!50003 SET character_set_client  = utf8mb4 */ ;
/*!50003 SET character_set_results = utf8mb4 */ ;
/*!50003 SET collation_connection  = utf8mb4_0900_ai_ci */ ;
/*!50003 SET @saved_sql_mode       = @@sql_mode */ ;
/*!50003 SET sql_mode              = '' */ ;
DELIMITER ;;
CREATE DEFINER=`root`@`localhost` PROCEDURE `CreateDistrict`(
    IN districtName VARCHAR(191),
    IN region ENUM('North','East','West','South')
)
BEGIN
    INSERT INTO districts (district_name, region) VALUES (districtName, region);
END ;;
DELIMITER ;
/*!50003 SET sql_mode              = @saved_sql_mode */ ;
/*!50003 SET character_set_client  = @saved_cs_client */ ;
/*!50003 SET character_set_results = @saved_cs_results */ ;
/*!50003 SET collation_connection  = @saved_col_connection */ ;
/*!50003 DROP PROCEDURE IF EXISTS `CreateEvent` */;
/*!50003 SET @saved_cs_client      = @@character_set_client */ ;
/*!50003 SET @saved_cs_results     = @@character_set_results */ ;
/*!50003 SET @saved_col_connection = @@collation_connection */ ;
/*!50003 SET character_set_client  = utf8mb4 */ ;
/*!50003 SET character_set_results = utf8mb4 */ ;
/*!50003 SET collation_connection  = utf8mb4_0900_ai_ci */ ;
/*!50003 SET @saved_sql_mode       = @@sql_mode */ ;
/*!50003 SET sql_mode              = '' */ ;
DELIMITER ;;
CREATE DEFINER=`root`@`localhost` PROCEDURE `CreateEvent`(
    IN p_event_name VARCHAR(255),
    IN p_description TEXT,
    IN p_start_date DATE,
    IN p_end_date DATE,
    IN p_location VARCHAR(255),
    IN p_committee_id INT,
    IN p_tags JSON,
    IN p_isVerifiable TINYINT,
    IN p_verification_status_id INT,
    IN p_is_active TINYINT
)
BEGIN
    INSERT INTO events (event_name, description, start_date, end_date, location, committee_id, tags, isVerifiable, verification_status_id, is_active)
    VALUES (p_event_name, p_description, p_start_date, p_end_date, p_location, p_committee_id, p_tags, p_isVerifiable, p_verification_status_id, p_is_active);
END ;;
DELIMITER ;
/*!50003 SET sql_mode              = @saved_sql_mode */ ;
/*!50003 SET character_set_client  = @saved_cs_client */ ;
/*!50003 SET character_set_results = @saved_cs_results */ ;
/*!50003 SET collation_connection  = @saved_col_connection */ ;
/*!50003 DROP PROCEDURE IF EXISTS `CreateHomestay` */;
/*!50003 SET @saved_cs_client      = @@character_set_client */ ;
/*!50003 SET @saved_cs_results     = @@character_set_results */ ;
/*!50003 SET @saved_col_connection = @@collation_connection */ ;
/*!50003 SET character_set_client  = utf8mb4 */ ;
/*!50003 SET character_set_results = utf8mb4 */ ;
/*!50003 SET collation_connection  = utf8mb4_0900_ai_ci */ ;
/*!50003 SET @saved_sql_mode       = @@sql_mode */ ;
/*!50003 SET sql_mode              = '' */ ;
DELIMITER ;;
CREATE DEFINER=`root`@`localhost` PROCEDURE `CreateHomestay`(
    IN p_homestay_name VARCHAR(255),
    IN p_committee_id INT,
    IN p_address TEXT,
    IN p_description TEXT, 
    IN p_owner_name VARCHAR(255),
    IN p_owner_mobile VARCHAR(20),
    IN p_total_rooms INT,
    IN p_room_tariff DECIMAL(10,2),
    IN p_tags JSON,
    IN p_amenities JSON,
    IN p_payment_methods VARCHAR(255),
    IN p_social_media_links JSON,
    IN p_isVerifiable TINYINT,
    IN p_verification_status_id INT,
    IN p_is_active TINYINT
)
BEGIN
    INSERT INTO homestays (
        homestay_name,
        committee_id,
        address,
        description, 
        owner_name,
        owner_mobile,
        total_rooms,
        room_tariff,
        tags,
        amenities,
        payment_methods,
        social_media_links,
        isVerifiable,
        verification_status_id,
        is_active
    ) VALUES (
        p_homestay_name,
        p_committee_id,
        p_address,
        p_description,
        p_owner_name,
        p_owner_mobile,
        p_total_rooms,
        p_room_tariff,
        p_tags,
        p_amenities,
        p_payment_methods,
        p_social_media_links,
        p_isVerifiable,
        p_verification_status_id,
        p_is_active
    );
END ;;
DELIMITER ;
/*!50003 SET sql_mode              = @saved_sql_mode */ ;
/*!50003 SET character_set_client  = @saved_cs_client */ ;
/*!50003 SET character_set_results = @saved_cs_results */ ;
/*!50003 SET collation_connection  = @saved_col_connection */ ;
/*!50003 DROP PROCEDURE IF EXISTS `CreateProduct` */;
/*!50003 SET @saved_cs_client      = @@character_set_client */ ;
/*!50003 SET @saved_cs_results     = @@character_set_results */ ;
/*!50003 SET @saved_col_connection = @@collation_connection */ ;
/*!50003 SET character_set_client  = utf8mb4 */ ;
/*!50003 SET character_set_results = utf8mb4 */ ;
/*!50003 SET collation_connection  = utf8mb4_0900_ai_ci */ ;
/*!50003 SET @saved_sql_mode       = @@sql_mode */ ;
/*!50003 SET sql_mode              = '' */ ;
DELIMITER ;;
CREATE DEFINER=`root`@`localhost` PROCEDURE `CreateProduct`(
    IN p_product_name VARCHAR(255),
    IN p_description TEXT,
    IN p_metric_unit VARCHAR(50),
    IN p_metric_value DECIMAL(10,2),
    IN p_price DECIMAL(10,2),
    IN p_committee_id INT,
    IN p_homestay_id INT,
    IN p_tags JSON,
    IN p_isVerifiable BOOLEAN,
    IN p_verification_status_id INT,
    IN p_is_active BOOLEAN
)
BEGIN
    INSERT INTO products (
        product_name, 
        description, 
        metric_unit, 
        metric_value, 
        price, 
        committee_id, 
        homestay_id, 
        tags, 
        isVerifiable, 
        verification_status_id, 
        is_active
    ) VALUES (
        p_product_name, 
        p_description, 
        IFNULL(p_metric_unit, 'Per Unit'), -- Default value if NULL
        IFNULL(p_metric_value, NULL), 
        p_price, 
        p_committee_id, 
        IFNULL(p_homestay_id, NULL), 
        IFNULL(p_tags, NULL), 
        p_isVerifiable, 
        p_verification_status_id, 
        p_is_active
    );
END ;;
DELIMITER ;
/*!50003 SET sql_mode              = @saved_sql_mode */ ;
/*!50003 SET character_set_client  = @saved_cs_client */ ;
/*!50003 SET character_set_results = @saved_cs_results */ ;
/*!50003 SET collation_connection  = @saved_col_connection */ ;
/*!50003 DROP PROCEDURE IF EXISTS `CreateUser` */;
/*!50003 SET @saved_cs_client      = @@character_set_client */ ;
/*!50003 SET @saved_cs_results     = @@character_set_results */ ;
/*!50003 SET @saved_col_connection = @@collation_connection */ ;
/*!50003 SET character_set_client  = utf8mb4 */ ;
/*!50003 SET character_set_results = utf8mb4 */ ;
/*!50003 SET collation_connection  = utf8mb4_0900_ai_ci */ ;
/*!50003 SET @saved_sql_mode       = @@sql_mode */ ;
/*!50003 SET sql_mode              = '' */ ;
DELIMITER ;;
CREATE DEFINER=`root`@`localhost` PROCEDURE `CreateUser`(
    IN p_Name VARCHAR(255),
    IN p_Email VARCHAR(255),
    IN p_Password VARCHAR(255),
    IN p_RoleId INT,
    IN p_CommitteeId INT,
    IN p_IsActive BOOLEAN
)
BEGIN
    INSERT INTO Users (name, email, password, role_id, committee_id, is_active)
    VALUES (p_Name, p_Email, p_Password, p_RoleId, p_CommitteeId, p_IsActive);
END ;;
DELIMITER ;
/*!50003 SET sql_mode              = @saved_sql_mode */ ;
/*!50003 SET character_set_client  = @saved_cs_client */ ;
/*!50003 SET character_set_results = @saved_cs_results */ ;
/*!50003 SET collation_connection  = @saved_col_connection */ ;
/*!50003 DROP PROCEDURE IF EXISTS `DeleteActivity` */;
/*!50003 SET @saved_cs_client      = @@character_set_client */ ;
/*!50003 SET @saved_cs_results     = @@character_set_results */ ;
/*!50003 SET @saved_col_connection = @@collation_connection */ ;
/*!50003 SET character_set_client  = utf8mb4 */ ;
/*!50003 SET character_set_results = utf8mb4 */ ;
/*!50003 SET collation_connection  = utf8mb4_0900_ai_ci */ ;
/*!50003 SET @saved_sql_mode       = @@sql_mode */ ;
/*!50003 SET sql_mode              = '' */ ;
DELIMITER ;;
CREATE DEFINER=`root`@`localhost` PROCEDURE `DeleteActivity`(
    IN p_activity_id INT
)
BEGIN
    DECLARE rows_affected INT;

    -- Check if the activity exists before deleting
    IF EXISTS (SELECT 1 FROM activities WHERE activity_id = p_activity_id) THEN
        DELETE FROM activities WHERE activity_id = p_activity_id;
        SET rows_affected = ROW_COUNT();
    ELSE
        SET rows_affected = 0; -- Activity does not exist
    END IF;

    -- ✅ Return 1 if delete was successful, 0 otherwise
    SELECT rows_affected AS success;
END ;;
DELIMITER ;
/*!50003 SET sql_mode              = @saved_sql_mode */ ;
/*!50003 SET character_set_client  = @saved_cs_client */ ;
/*!50003 SET character_set_results = @saved_cs_results */ ;
/*!50003 SET collation_connection  = @saved_col_connection */ ;
/*!50003 DROP PROCEDURE IF EXISTS `DeleteCommittee` */;
/*!50003 SET @saved_cs_client      = @@character_set_client */ ;
/*!50003 SET @saved_cs_results     = @@character_set_results */ ;
/*!50003 SET @saved_col_connection = @@collation_connection */ ;
/*!50003 SET character_set_client  = utf8mb4 */ ;
/*!50003 SET character_set_results = utf8mb4 */ ;
/*!50003 SET collation_connection  = utf8mb4_0900_ai_ci */ ;
/*!50003 SET @saved_sql_mode       = @@sql_mode */ ;
/*!50003 SET sql_mode              = '' */ ;
DELIMITER ;;
CREATE DEFINER=`root`@`localhost` PROCEDURE `DeleteCommittee`(IN p_committee_id INT)
BEGIN
    DELETE FROM committees WHERE committee_id = p_committee_id;
END ;;
DELIMITER ;
/*!50003 SET sql_mode              = @saved_sql_mode */ ;
/*!50003 SET character_set_client  = @saved_cs_client */ ;
/*!50003 SET character_set_results = @saved_cs_results */ ;
/*!50003 SET collation_connection  = @saved_col_connection */ ;
/*!50003 DROP PROCEDURE IF EXISTS `DeleteEvent` */;
/*!50003 SET @saved_cs_client      = @@character_set_client */ ;
/*!50003 SET @saved_cs_results     = @@character_set_results */ ;
/*!50003 SET @saved_col_connection = @@collation_connection */ ;
/*!50003 SET character_set_client  = utf8mb4 */ ;
/*!50003 SET character_set_results = utf8mb4 */ ;
/*!50003 SET collation_connection  = utf8mb4_0900_ai_ci */ ;
/*!50003 SET @saved_sql_mode       = @@sql_mode */ ;
/*!50003 SET sql_mode              = '' */ ;
DELIMITER ;;
CREATE DEFINER=`root`@`localhost` PROCEDURE `DeleteEvent`(IN p_event_id INT)
BEGIN
    DELETE FROM events WHERE event_id = p_event_id;
END ;;
DELIMITER ;
/*!50003 SET sql_mode              = @saved_sql_mode */ ;
/*!50003 SET character_set_client  = @saved_cs_client */ ;
/*!50003 SET character_set_results = @saved_cs_results */ ;
/*!50003 SET collation_connection  = @saved_col_connection */ ;
/*!50003 DROP PROCEDURE IF EXISTS `DeleteHomestay` */;
/*!50003 SET @saved_cs_client      = @@character_set_client */ ;
/*!50003 SET @saved_cs_results     = @@character_set_results */ ;
/*!50003 SET @saved_col_connection = @@collation_connection */ ;
/*!50003 SET character_set_client  = utf8mb4 */ ;
/*!50003 SET character_set_results = utf8mb4 */ ;
/*!50003 SET collation_connection  = utf8mb4_0900_ai_ci */ ;
/*!50003 SET @saved_sql_mode       = @@sql_mode */ ;
/*!50003 SET sql_mode              = '' */ ;
DELIMITER ;;
CREATE DEFINER=`root`@`localhost` PROCEDURE `DeleteHomestay`(IN p_homestay_id INT)
BEGIN
    DELETE FROM homestays WHERE homestay_id = p_homestay_id;
END ;;
DELIMITER ;
/*!50003 SET sql_mode              = @saved_sql_mode */ ;
/*!50003 SET character_set_client  = @saved_cs_client */ ;
/*!50003 SET character_set_results = @saved_cs_results */ ;
/*!50003 SET collation_connection  = @saved_col_connection */ ;
/*!50003 DROP PROCEDURE IF EXISTS `DeleteImage` */;
/*!50003 SET @saved_cs_client      = @@character_set_client */ ;
/*!50003 SET @saved_cs_results     = @@character_set_results */ ;
/*!50003 SET @saved_col_connection = @@collation_connection */ ;
/*!50003 SET character_set_client  = utf8mb4 */ ;
/*!50003 SET character_set_results = utf8mb4 */ ;
/*!50003 SET collation_connection  = utf8mb4_0900_ai_ci */ ;
/*!50003 SET @saved_sql_mode       = @@sql_mode */ ;
/*!50003 SET sql_mode              = '' */ ;
DELIMITER ;;
CREATE DEFINER=`root`@`localhost` PROCEDURE `DeleteImage`(
    IN p_image_id INT
)
BEGIN
    DELETE FROM images WHERE image_id = p_image_id;
END ;;
DELIMITER ;
/*!50003 SET sql_mode              = @saved_sql_mode */ ;
/*!50003 SET character_set_client  = @saved_cs_client */ ;
/*!50003 SET character_set_results = @saved_cs_results */ ;
/*!50003 SET collation_connection  = @saved_col_connection */ ;
/*!50003 DROP PROCEDURE IF EXISTS `DeleteProduct` */;
/*!50003 SET @saved_cs_client      = @@character_set_client */ ;
/*!50003 SET @saved_cs_results     = @@character_set_results */ ;
/*!50003 SET @saved_col_connection = @@collation_connection */ ;
/*!50003 SET character_set_client  = utf8mb4 */ ;
/*!50003 SET character_set_results = utf8mb4 */ ;
/*!50003 SET collation_connection  = utf8mb4_0900_ai_ci */ ;
/*!50003 SET @saved_sql_mode       = @@sql_mode */ ;
/*!50003 SET sql_mode              = '' */ ;
DELIMITER ;;
CREATE DEFINER=`root`@`localhost` PROCEDURE `DeleteProduct`(IN p_product_id INT)
BEGIN
    DELETE FROM products WHERE product_id = p_product_id;
END ;;
DELIMITER ;
/*!50003 SET sql_mode              = @saved_sql_mode */ ;
/*!50003 SET character_set_client  = @saved_cs_client */ ;
/*!50003 SET character_set_results = @saved_cs_results */ ;
/*!50003 SET collation_connection  = @saved_col_connection */ ;
/*!50003 DROP PROCEDURE IF EXISTS `DeleteUser` */;
/*!50003 SET @saved_cs_client      = @@character_set_client */ ;
/*!50003 SET @saved_cs_results     = @@character_set_results */ ;
/*!50003 SET @saved_col_connection = @@collation_connection */ ;
/*!50003 SET character_set_client  = utf8mb4 */ ;
/*!50003 SET character_set_results = utf8mb4 */ ;
/*!50003 SET collation_connection  = utf8mb4_0900_ai_ci */ ;
/*!50003 SET @saved_sql_mode       = @@sql_mode */ ;
/*!50003 SET sql_mode              = '' */ ;
DELIMITER ;;
CREATE DEFINER=`root`@`localhost` PROCEDURE `DeleteUser`(IN p_UserId INT)
BEGIN
    DELETE FROM Users WHERE user_id = p_UserId;
END ;;
DELIMITER ;
/*!50003 SET sql_mode              = @saved_sql_mode */ ;
/*!50003 SET character_set_client  = @saved_cs_client */ ;
/*!50003 SET character_set_results = @saved_cs_results */ ;
/*!50003 SET collation_connection  = @saved_col_connection */ ;
/*!50003 DROP PROCEDURE IF EXISTS `FilterData` */;
/*!50003 SET @saved_cs_client      = @@character_set_client */ ;
/*!50003 SET @saved_cs_results     = @@character_set_results */ ;
/*!50003 SET @saved_col_connection = @@collation_connection */ ;
/*!50003 SET character_set_client  = utf8mb4 */ ;
/*!50003 SET character_set_results = utf8mb4 */ ;
/*!50003 SET collation_connection  = utf8mb4_0900_ai_ci */ ;
/*!50003 SET @saved_sql_mode       = @@sql_mode */ ;
/*!50003 SET sql_mode              = '' */ ;
DELIMITER ;;
CREATE DEFINER=`root`@`localhost` PROCEDURE `FilterData`(
    IN p_category VARCHAR(50),
    IN p_district_id INT,  
    IN p_village_id INT,  
    IN p_search TEXT  
)
BEGIN
    -- Declare variables
    SET @sql_query = '';
    SET @where_clause = ' WHERE 1=1 ';
    SET @search_column = '';

    -- Select the correct table and join with committees for district_id
    IF p_category = 'Villages' THEN
        SET @sql_query = 'SELECT c.committee_id AS id, c.committee_name AS name, c.district_id, c.verification_status_id 
                          FROM committees c';
        SET @search_column = 'c.committee_name';
    ELSEIF p_category = 'Homestays' THEN
        SET @sql_query = 'SELECT h.homestay_id AS id, h.homestay_name AS name, c.district_id, h.committee_id, h.verification_status_id 
                          FROM homestays h 
                          JOIN committees c ON h.committee_id = c.committee_id';
        SET @search_column = 'h.homestay_name';
    ELSEIF p_category = 'Activities' THEN
        SET @sql_query = 'SELECT a.activity_id AS id, a.activity_name AS name, c.district_id, a.committee_id, a.verification_status_id 
                          FROM activities a 
                          JOIN committees c ON a.committee_id = c.committee_id';
        SET @search_column = 'a.activity_name';
    ELSEIF p_category = 'Products' THEN
        SET @sql_query = 'SELECT p.product_id AS id, p.product_name AS name, c.district_id, p.committee_id, p.verification_status_id 
                          FROM products p 
                          JOIN committees c ON p.committee_id = c.committee_id';
        SET @search_column = 'p.product_name';
    ELSEIF p_category = 'Events' THEN
        SET @sql_query = 'SELECT e.event_id AS id, e.event_name AS name, c.district_id, e.committee_id, e.verification_status_id 
                          FROM events e 
                          JOIN committees c ON e.committee_id = c.committee_id';
        SET @search_column = 'e.event_name';
    ELSE
        SIGNAL SQLSTATE '45000' SET MESSAGE_TEXT = 'Invalid category';
    END IF;

    -- Apply filters dynamically
    IF p_district_id IS NOT NULL THEN
        SET @where_clause = CONCAT(@where_clause, ' AND c.district_id = ', p_district_id);
    END IF;

    IF p_village_id IS NOT NULL THEN
        SET @where_clause = CONCAT(@where_clause, ' AND c.committee_id = ', p_village_id);
    END IF;

    IF p_search IS NOT NULL AND p_search <> '' THEN
        SET @where_clause = CONCAT(@where_clause, " AND (LOWER(", @search_column, ") LIKE LOWER(CONCAT('%", p_search, "%')))");
    END IF;

    -- Combine query with filters
    SET @sql_query = CONCAT(@sql_query, @where_clause);

    -- Debugging: Print query (for testing only, remove in production)
    -- SELECT @sql_query;

    -- Prepare and execute statement
    PREPARE stmt FROM @sql_query;
    EXECUTE stmt;
    DEALLOCATE PREPARE stmt;
END ;;
DELIMITER ;
/*!50003 SET sql_mode              = @saved_sql_mode */ ;
/*!50003 SET character_set_client  = @saved_cs_client */ ;
/*!50003 SET character_set_results = @saved_cs_results */ ;
/*!50003 SET collation_connection  = @saved_col_connection */ ;
/*!50003 DROP PROCEDURE IF EXISTS `GetActivityById` */;
/*!50003 SET @saved_cs_client      = @@character_set_client */ ;
/*!50003 SET @saved_cs_results     = @@character_set_results */ ;
/*!50003 SET @saved_col_connection = @@collation_connection */ ;
/*!50003 SET character_set_client  = utf8mb4 */ ;
/*!50003 SET character_set_results = utf8mb4 */ ;
/*!50003 SET collation_connection  = utf8mb4_0900_ai_ci */ ;
/*!50003 SET @saved_sql_mode       = @@sql_mode */ ;
/*!50003 SET sql_mode              = '' */ ;
DELIMITER ;;
CREATE DEFINER=`root`@`localhost` PROCEDURE `GetActivityById`(IN p_activity_id INT)
BEGIN
    SELECT 
        a.activity_id, 
        a.activity_name, 
        a.description,
        a.tags,  -- Fetching tags (JSON format)
        a.committee_id,
        c.district_id,  -- Fetch district_id from committees
        d.district_name,  -- Fetch district_name from districts
        d.region,  -- Fetch region from districts
        a.homestay_id,
        a.isVerifiable,
        a.verification_status_id,
        mvs.status_type,  -- Fetch verification status type from master_verification_status
        a.is_active
    FROM activities a
    -- Join committees table to get district_id
    JOIN committees c ON a.committee_id = c.committee_id
    -- Join districts table to get district_name and region
    JOIN districts d ON c.district_id = d.district_id
    -- Left join master_verification_status table to get status type
    LEFT JOIN master_verification_status mvs ON a.verification_status_id = mvs.verification_status_id
    WHERE a.activity_id = p_activity_id;  -- Filter by activity_id
END ;;
DELIMITER ;
/*!50003 SET sql_mode              = @saved_sql_mode */ ;
/*!50003 SET character_set_client  = @saved_cs_client */ ;
/*!50003 SET character_set_results = @saved_cs_results */ ;
/*!50003 SET collation_connection  = @saved_col_connection */ ;
/*!50003 DROP PROCEDURE IF EXISTS `GetAllActivities` */;
/*!50003 SET @saved_cs_client      = @@character_set_client */ ;
/*!50003 SET @saved_cs_results     = @@character_set_results */ ;
/*!50003 SET @saved_col_connection = @@collation_connection */ ;
/*!50003 SET character_set_client  = utf8mb4 */ ;
/*!50003 SET character_set_results = utf8mb4 */ ;
/*!50003 SET collation_connection  = utf8mb4_0900_ai_ci */ ;
/*!50003 SET @saved_sql_mode       = @@sql_mode */ ;
/*!50003 SET sql_mode              = '' */ ;
DELIMITER ;;
CREATE DEFINER=`root`@`localhost` PROCEDURE `GetAllActivities`()
BEGIN
    SELECT 
        a.activity_id, 
        a.activity_name, 
        a.description,
        a.tags,  -- Fetching tags (JSON format)
        a.committee_id,
        c.district_id,  -- Fetch district_id from committees
        d.district_name,  -- Fetch district_name from districts
        d.region,  -- Fetch region from districts
        a.homestay_id,
        a.isVerifiable,
        a.verification_status_id,
        mvs.status_type,  -- Fetch verification status type from master_verification_status
        a.is_active
    FROM activities a
    -- Join committees table to get district_id
    JOIN committees c ON a.committee_id = c.committee_id
    -- Join districts table to get district_name and region
    JOIN districts d ON c.district_id = d.district_id
    -- Left join master_verification_status table to get status type
    LEFT JOIN master_verification_status mvs ON a.verification_status_id = mvs.verification_status_id;
END ;;
DELIMITER ;
/*!50003 SET sql_mode              = @saved_sql_mode */ ;
/*!50003 SET character_set_client  = @saved_cs_client */ ;
/*!50003 SET character_set_results = @saved_cs_results */ ;
/*!50003 SET collation_connection  = @saved_col_connection */ ;
/*!50003 DROP PROCEDURE IF EXISTS `GetAllCommittees` */;
/*!50003 SET @saved_cs_client      = @@character_set_client */ ;
/*!50003 SET @saved_cs_results     = @@character_set_results */ ;
/*!50003 SET @saved_col_connection = @@collation_connection */ ;
/*!50003 SET character_set_client  = utf8mb4 */ ;
/*!50003 SET character_set_results = utf8mb4 */ ;
/*!50003 SET collation_connection  = utf8mb4_0900_ai_ci */ ;
/*!50003 SET @saved_sql_mode       = @@sql_mode */ ;
/*!50003 SET sql_mode              = '' */ ;
DELIMITER ;;
CREATE DEFINER=`root`@`localhost` PROCEDURE `GetAllCommittees`()
BEGIN
    SELECT 
        c.committee_id,
        c.committee_name,
        c.description,
        
        c.district_id,
        d.district_name,
        
        c.verification_status_id,
        mvs.status_type AS verification_status,

        c.contact_number,
        c.email,
        c.address,
        c.tags,
        c.tourist_attractions,
        c.latitude,
        c.longitude,
        c.leadership,
        c.isVerifiable,
        c.is_active
    FROM committees c
    LEFT JOIN districts d ON c.district_id = d.district_id
    LEFT JOIN master_verification_status mvs ON c.verification_status_id = mvs.verification_status_id;
END ;;
DELIMITER ;
/*!50003 SET sql_mode              = @saved_sql_mode */ ;
/*!50003 SET character_set_client  = @saved_cs_client */ ;
/*!50003 SET character_set_results = @saved_cs_results */ ;
/*!50003 SET collation_connection  = @saved_col_connection */ ;
/*!50003 DROP PROCEDURE IF EXISTS `GetAllDistricts` */;
/*!50003 SET @saved_cs_client      = @@character_set_client */ ;
/*!50003 SET @saved_cs_results     = @@character_set_results */ ;
/*!50003 SET @saved_col_connection = @@collation_connection */ ;
/*!50003 SET character_set_client  = utf8mb4 */ ;
/*!50003 SET character_set_results = utf8mb4 */ ;
/*!50003 SET collation_connection  = utf8mb4_0900_ai_ci */ ;
/*!50003 SET @saved_sql_mode       = @@sql_mode */ ;
/*!50003 SET sql_mode              = '' */ ;
DELIMITER ;;
CREATE DEFINER=`root`@`localhost` PROCEDURE `GetAllDistricts`()
BEGIN
    SELECT * FROM districts;
END ;;
DELIMITER ;
/*!50003 SET sql_mode              = @saved_sql_mode */ ;
/*!50003 SET character_set_client  = @saved_cs_client */ ;
/*!50003 SET character_set_results = @saved_cs_results */ ;
/*!50003 SET collation_connection  = @saved_col_connection */ ;
/*!50003 DROP PROCEDURE IF EXISTS `GetAllEvents` */;
/*!50003 SET @saved_cs_client      = @@character_set_client */ ;
/*!50003 SET @saved_cs_results     = @@character_set_results */ ;
/*!50003 SET @saved_col_connection = @@collation_connection */ ;
/*!50003 SET character_set_client  = utf8mb4 */ ;
/*!50003 SET character_set_results = utf8mb4 */ ;
/*!50003 SET collation_connection  = utf8mb4_0900_ai_ci */ ;
/*!50003 SET @saved_sql_mode       = @@sql_mode */ ;
/*!50003 SET sql_mode              = '' */ ;
DELIMITER ;;
CREATE DEFINER=`root`@`localhost` PROCEDURE `GetAllEvents`()
BEGIN
    SELECT * FROM events;
END ;;
DELIMITER ;
/*!50003 SET sql_mode              = @saved_sql_mode */ ;
/*!50003 SET character_set_client  = @saved_cs_client */ ;
/*!50003 SET character_set_results = @saved_cs_results */ ;
/*!50003 SET collation_connection  = @saved_col_connection */ ;
/*!50003 DROP PROCEDURE IF EXISTS `GetAllHomestays` */;
/*!50003 SET @saved_cs_client      = @@character_set_client */ ;
/*!50003 SET @saved_cs_results     = @@character_set_results */ ;
/*!50003 SET @saved_col_connection = @@collation_connection */ ;
/*!50003 SET character_set_client  = utf8mb4 */ ;
/*!50003 SET character_set_results = utf8mb4 */ ;
/*!50003 SET collation_connection  = utf8mb4_0900_ai_ci */ ;
/*!50003 SET @saved_sql_mode       = @@sql_mode */ ;
/*!50003 SET sql_mode              = '' */ ;
DELIMITER ;;
CREATE DEFINER=`root`@`localhost` PROCEDURE `GetAllHomestays`()
BEGIN
    SELECT 
        h.homestay_id,
        h.homestay_name,
        h.committee_id,
        c.district_id,
        d.district_name,  -- Fetching district_name from districts table
        d.region,         -- Fetching region from districts table
        h.address,
        h.description,
        h.owner_name,
        h.owner_mobile,
        h.total_rooms,
        h.room_tariff,
        h.tags,
        h.amenities,
        h.payment_methods,
        h.social_media_links,
        h.isVerifiable,
        h.verification_status_id,
        v.status_type,     -- Fetching status_type from master_verification_status
        h.is_active
    FROM homestays h
    JOIN committees c ON h.committee_id = c.committee_id
    JOIN districts d ON c.district_id = d.district_id
    LEFT JOIN master_verification_status v ON h.verification_status_id = v.verification_status_id; -- Joining to get status_type
END ;;
DELIMITER ;
/*!50003 SET sql_mode              = @saved_sql_mode */ ;
/*!50003 SET character_set_client  = @saved_cs_client */ ;
/*!50003 SET character_set_results = @saved_cs_results */ ;
/*!50003 SET collation_connection  = @saved_col_connection */ ;
/*!50003 DROP PROCEDURE IF EXISTS `GetAllProducts` */;
/*!50003 SET @saved_cs_client      = @@character_set_client */ ;
/*!50003 SET @saved_cs_results     = @@character_set_results */ ;
/*!50003 SET @saved_col_connection = @@collation_connection */ ;
/*!50003 SET character_set_client  = utf8mb4 */ ;
/*!50003 SET character_set_results = utf8mb4 */ ;
/*!50003 SET collation_connection  = utf8mb4_0900_ai_ci */ ;
/*!50003 SET @saved_sql_mode       = @@sql_mode */ ;
/*!50003 SET sql_mode              = '' */ ;
DELIMITER ;;
CREATE DEFINER=`root`@`localhost` PROCEDURE `GetAllProducts`()
BEGIN
    SELECT 
        -- Product Details
        p.product_id, 
        p.product_name, 
        p.description, 
        p.metric_unit,
        p.metric_value,
        p.price, 
        
        -- Committee & District Details
        p.committee_id, 
        c.committee_name, 
        c.district_id, 
        d.district_name, 
        d.region, 
        
        -- Homestay Details (if applicable)
        p.homestay_id, 
        IFNULL(h.homestay_name, NULL) AS homestay_name,  -- Ensures NULL when homestay_id is NULL
        
        -- Verification & Status
        p.tags, 
        p.isVerifiable, 
        p.verification_status_id, 
        v.status_type,  -- Fetching status_type from master_verification_status
        p.is_active
        
    FROM products p
    -- Joining Committees to fetch committee_name & district_id
    JOIN committees c ON p.committee_id = c.committee_id
    -- Joining Districts to fetch district_name & region
    JOIN districts d ON c.district_id = d.district_id
    -- Joining Homestays to fetch homestay_name (if applicable)
    LEFT JOIN homestays h ON p.homestay_id = h.homestay_id
    -- Joining Verification Status to fetch status_type
    LEFT JOIN master_verification_status v ON p.verification_status_id = v.verification_status_id;
END ;;
DELIMITER ;
/*!50003 SET sql_mode              = @saved_sql_mode */ ;
/*!50003 SET character_set_client  = @saved_cs_client */ ;
/*!50003 SET character_set_results = @saved_cs_results */ ;
/*!50003 SET collation_connection  = @saved_col_connection */ ;
/*!50003 DROP PROCEDURE IF EXISTS `GetAllUsers` */;
/*!50003 SET @saved_cs_client      = @@character_set_client */ ;
/*!50003 SET @saved_cs_results     = @@character_set_results */ ;
/*!50003 SET @saved_col_connection = @@collation_connection */ ;
/*!50003 SET character_set_client  = utf8mb4 */ ;
/*!50003 SET character_set_results = utf8mb4 */ ;
/*!50003 SET collation_connection  = utf8mb4_0900_ai_ci */ ;
/*!50003 SET @saved_sql_mode       = @@sql_mode */ ;
/*!50003 SET sql_mode              = '' */ ;
DELIMITER ;;
CREATE DEFINER=`root`@`localhost` PROCEDURE `GetAllUsers`()
BEGIN
    SELECT * FROM Users;
END ;;
DELIMITER ;
/*!50003 SET sql_mode              = @saved_sql_mode */ ;
/*!50003 SET character_set_client  = @saved_cs_client */ ;
/*!50003 SET character_set_results = @saved_cs_results */ ;
/*!50003 SET collation_connection  = @saved_col_connection */ ;
/*!50003 DROP PROCEDURE IF EXISTS `GetAllVillageNames` */;
/*!50003 SET @saved_cs_client      = @@character_set_client */ ;
/*!50003 SET @saved_cs_results     = @@character_set_results */ ;
/*!50003 SET @saved_col_connection = @@collation_connection */ ;
/*!50003 SET character_set_client  = utf8mb4 */ ;
/*!50003 SET character_set_results = utf8mb4 */ ;
/*!50003 SET collation_connection  = utf8mb4_0900_ai_ci */ ;
/*!50003 SET @saved_sql_mode       = @@sql_mode */ ;
/*!50003 SET sql_mode              = '' */ ;
DELIMITER ;;
CREATE DEFINER=`root`@`localhost` PROCEDURE `GetAllVillageNames`()
BEGIN
    SELECT committee_id, committee_name, district_id, verification_status_id, is_active 
    FROM committees;
END ;;
DELIMITER ;
/*!50003 SET sql_mode              = @saved_sql_mode */ ;
/*!50003 SET character_set_client  = @saved_cs_client */ ;
/*!50003 SET character_set_results = @saved_cs_results */ ;
/*!50003 SET collation_connection  = @saved_col_connection */ ;
/*!50003 DROP PROCEDURE IF EXISTS `GetCommitteeById` */;
/*!50003 SET @saved_cs_client      = @@character_set_client */ ;
/*!50003 SET @saved_cs_results     = @@character_set_results */ ;
/*!50003 SET @saved_col_connection = @@collation_connection */ ;
/*!50003 SET character_set_client  = utf8mb4 */ ;
/*!50003 SET character_set_results = utf8mb4 */ ;
/*!50003 SET collation_connection  = utf8mb4_0900_ai_ci */ ;
/*!50003 SET @saved_sql_mode       = @@sql_mode */ ;
/*!50003 SET sql_mode              = '' */ ;
DELIMITER ;;
CREATE DEFINER=`root`@`localhost` PROCEDURE `GetCommitteeById`(IN p_committee_id INT)
BEGIN
    SELECT 
        c.committee_id,
        c.committee_name,
        c.description,
        
        c.district_id,
        d.district_name,
        
        c.verification_status_id,
        mvs.status_type AS verification_status,

        c.contact_number,
        c.email,
        c.address,
        c.tags,
        c.tourist_attractions,
        c.latitude,
        c.longitude,
        c.leadership,
        c.isVerifiable,
        c.is_active
    FROM committees c
    LEFT JOIN districts d ON c.district_id = d.district_id
    LEFT JOIN master_verification_status mvs ON c.verification_status_id = mvs.verification_status_id
    WHERE c.committee_id = p_committee_id;
END ;;
DELIMITER ;
/*!50003 SET sql_mode              = @saved_sql_mode */ ;
/*!50003 SET character_set_client  = @saved_cs_client */ ;
/*!50003 SET character_set_results = @saved_cs_results */ ;
/*!50003 SET collation_connection  = @saved_col_connection */ ;
/*!50003 DROP PROCEDURE IF EXISTS `GetDistrictById` */;
/*!50003 SET @saved_cs_client      = @@character_set_client */ ;
/*!50003 SET @saved_cs_results     = @@character_set_results */ ;
/*!50003 SET @saved_col_connection = @@collation_connection */ ;
/*!50003 SET character_set_client  = utf8mb4 */ ;
/*!50003 SET character_set_results = utf8mb4 */ ;
/*!50003 SET collation_connection  = utf8mb4_0900_ai_ci */ ;
/*!50003 SET @saved_sql_mode       = @@sql_mode */ ;
/*!50003 SET sql_mode              = '' */ ;
DELIMITER ;;
CREATE DEFINER=`root`@`localhost` PROCEDURE `GetDistrictById`(IN districtId INT)
BEGIN
    SELECT * FROM districts WHERE district_id = districtId;
END ;;
DELIMITER ;
/*!50003 SET sql_mode              = @saved_sql_mode */ ;
/*!50003 SET character_set_client  = @saved_cs_client */ ;
/*!50003 SET character_set_results = @saved_cs_results */ ;
/*!50003 SET collation_connection  = @saved_col_connection */ ;
/*!50003 DROP PROCEDURE IF EXISTS `GetEntityCommitteeIdForImage` */;
/*!50003 SET @saved_cs_client      = @@character_set_client */ ;
/*!50003 SET @saved_cs_results     = @@character_set_results */ ;
/*!50003 SET @saved_col_connection = @@collation_connection */ ;
/*!50003 SET character_set_client  = utf8mb4 */ ;
/*!50003 SET character_set_results = utf8mb4 */ ;
/*!50003 SET collation_connection  = utf8mb4_0900_ai_ci */ ;
/*!50003 SET @saved_sql_mode       = @@sql_mode */ ;
/*!50003 SET sql_mode              = '' */ ;
DELIMITER ;;
CREATE DEFINER=`root`@`localhost` PROCEDURE `GetEntityCommitteeIdForImage`(
    IN p_entity_id INT,
    IN p_entity_type ENUM('Committee','Homestay','Activity','Event','Product')
)
BEGIN
    CASE p_entity_type
		WHEN 'Committee' THEN 
		SELECT committee_id FROM committees WHERE committee_id = p_entity_id;
        WHEN 'Homestay' THEN 
            SELECT committee_id FROM homestays WHERE homestay_id = p_entity_id;
        WHEN 'Activity' THEN 
            SELECT committee_id FROM activities WHERE activity_id = p_entity_id;
        WHEN 'Event' THEN 
            SELECT committee_id FROM events WHERE event_id = p_entity_id;
        WHEN 'Product' THEN 
            SELECT committee_id FROM products WHERE product_id = p_entity_id;
        ELSE 
            SELECT NULL;
    END CASE;
END ;;
DELIMITER ;
/*!50003 SET sql_mode              = @saved_sql_mode */ ;
/*!50003 SET character_set_client  = @saved_cs_client */ ;
/*!50003 SET character_set_results = @saved_cs_results */ ;
/*!50003 SET collation_connection  = @saved_col_connection */ ;
/*!50003 DROP PROCEDURE IF EXISTS `GetEntityCounts` */;
/*!50003 SET @saved_cs_client      = @@character_set_client */ ;
/*!50003 SET @saved_cs_results     = @@character_set_results */ ;
/*!50003 SET @saved_col_connection = @@collation_connection */ ;
/*!50003 SET character_set_client  = utf8mb4 */ ;
/*!50003 SET character_set_results = utf8mb4 */ ;
/*!50003 SET collation_connection  = utf8mb4_0900_ai_ci */ ;
/*!50003 SET @saved_sql_mode       = @@sql_mode */ ;
/*!50003 SET sql_mode              = '' */ ;
DELIMITER ;;
CREATE DEFINER=`root`@`localhost` PROCEDURE `GetEntityCounts`()
BEGIN
    DECLARE village_count INT DEFAULT 0;
    DECLARE homestay_count INT DEFAULT 0;
    DECLARE activity_count INT DEFAULT 0;
    DECLARE product_count INT DEFAULT 0;
    DECLARE event_count INT DEFAULT 0;

    -- Get total counts
    SELECT COUNT(*) INTO village_count FROM committees;
    SELECT COUNT(*) INTO homestay_count FROM homestays;
    SELECT COUNT(*) INTO activity_count FROM activities;
    SELECT COUNT(*) INTO product_count FROM products;
    SELECT COUNT(*) INTO event_count FROM events;

    -- Return results
    SELECT 
        village_count AS `Village Communities`,
        homestay_count AS `Home Stays`,
        activity_count AS `Things to Do`,
        product_count AS `Local Products`,
        event_count AS `Events`;
END ;;
DELIMITER ;
/*!50003 SET sql_mode              = @saved_sql_mode */ ;
/*!50003 SET character_set_client  = @saved_cs_client */ ;
/*!50003 SET character_set_results = @saved_cs_results */ ;
/*!50003 SET collation_connection  = @saved_col_connection */ ;
/*!50003 DROP PROCEDURE IF EXISTS `GetEventById` */;
/*!50003 SET @saved_cs_client      = @@character_set_client */ ;
/*!50003 SET @saved_cs_results     = @@character_set_results */ ;
/*!50003 SET @saved_col_connection = @@collation_connection */ ;
/*!50003 SET character_set_client  = utf8mb4 */ ;
/*!50003 SET character_set_results = utf8mb4 */ ;
/*!50003 SET collation_connection  = utf8mb4_0900_ai_ci */ ;
/*!50003 SET @saved_sql_mode       = @@sql_mode */ ;
/*!50003 SET sql_mode              = '' */ ;
DELIMITER ;;
CREATE DEFINER=`root`@`localhost` PROCEDURE `GetEventById`(IN p_event_id INT)
BEGIN
    SELECT * FROM events WHERE event_id = p_event_id;
END ;;
DELIMITER ;
/*!50003 SET sql_mode              = @saved_sql_mode */ ;
/*!50003 SET character_set_client  = @saved_cs_client */ ;
/*!50003 SET character_set_results = @saved_cs_results */ ;
/*!50003 SET collation_connection  = @saved_col_connection */ ;
/*!50003 DROP PROCEDURE IF EXISTS `GetHomestayById` */;
/*!50003 SET @saved_cs_client      = @@character_set_client */ ;
/*!50003 SET @saved_cs_results     = @@character_set_results */ ;
/*!50003 SET @saved_col_connection = @@collation_connection */ ;
/*!50003 SET character_set_client  = utf8mb4 */ ;
/*!50003 SET character_set_results = utf8mb4 */ ;
/*!50003 SET collation_connection  = utf8mb4_0900_ai_ci */ ;
/*!50003 SET @saved_sql_mode       = @@sql_mode */ ;
/*!50003 SET sql_mode              = '' */ ;
DELIMITER ;;
CREATE DEFINER=`root`@`localhost` PROCEDURE `GetHomestayById`(IN p_homestay_id INT)
BEGIN
    SELECT 
        h.homestay_id,
        h.homestay_name,
        h.committee_id,
        c.district_id,
        d.district_name,  -- Fetching district_name from districts table
        d.region,         -- Fetching region from districts table
        h.address,
        h.description,
        h.owner_name,
        h.owner_mobile,
        h.total_rooms,
        h.room_tariff,
        h.tags,
        h.amenities,
        h.payment_methods,
        h.social_media_links,
        h.isVerifiable,
        h.verification_status_id,
        v.status_type,     -- Fetching status_type from master_verification_status
        h.is_active
    FROM homestays h
    JOIN committees c ON h.committee_id = c.committee_id
    JOIN districts d ON c.district_id = d.district_id
    LEFT JOIN master_verification_status v ON h.verification_status_id = v.verification_status_id -- Joining to get status_type
    WHERE h.homestay_id = p_homestay_id;
END ;;
DELIMITER ;
/*!50003 SET sql_mode              = @saved_sql_mode */ ;
/*!50003 SET character_set_client  = @saved_cs_client */ ;
/*!50003 SET character_set_results = @saved_cs_results */ ;
/*!50003 SET collation_connection  = @saved_col_connection */ ;
/*!50003 DROP PROCEDURE IF EXISTS `GetImageById` */;
/*!50003 SET @saved_cs_client      = @@character_set_client */ ;
/*!50003 SET @saved_cs_results     = @@character_set_results */ ;
/*!50003 SET @saved_col_connection = @@collation_connection */ ;
/*!50003 SET character_set_client  = utf8mb4 */ ;
/*!50003 SET character_set_results = utf8mb4 */ ;
/*!50003 SET collation_connection  = utf8mb4_0900_ai_ci */ ;
/*!50003 SET @saved_sql_mode       = @@sql_mode */ ;
/*!50003 SET sql_mode              = '' */ ;
DELIMITER ;;
CREATE DEFINER=`root`@`localhost` PROCEDURE `GetImageById`(IN p_image_id INT)
BEGIN
    SELECT 
        i.image_id,
        i.image_url,
        i.original_image_name,
        i.image_name,
        i.file_size,
        i.mime_type,
        i.entity_type,
        i.entity_id,
        i.committee_id,
        i.uploaded_by,
        i.is_profile_image,
        i.status,
        i.created_at,
        i.updated_at
    FROM images i
    WHERE i.image_id = p_image_id;
END ;;
DELIMITER ;
/*!50003 SET sql_mode              = @saved_sql_mode */ ;
/*!50003 SET character_set_client  = @saved_cs_client */ ;
/*!50003 SET character_set_results = @saved_cs_results */ ;
/*!50003 SET collation_connection  = @saved_col_connection */ ;
/*!50003 DROP PROCEDURE IF EXISTS `GetImagesByEntity` */;
/*!50003 SET @saved_cs_client      = @@character_set_client */ ;
/*!50003 SET @saved_cs_results     = @@character_set_results */ ;
/*!50003 SET @saved_col_connection = @@collation_connection */ ;
/*!50003 SET character_set_client  = utf8mb4 */ ;
/*!50003 SET character_set_results = utf8mb4 */ ;
/*!50003 SET collation_connection  = utf8mb4_0900_ai_ci */ ;
/*!50003 SET @saved_sql_mode       = @@sql_mode */ ;
/*!50003 SET sql_mode              = '' */ ;
DELIMITER ;;
CREATE DEFINER=`root`@`localhost` PROCEDURE `GetImagesByEntity`(
    IN p_entity_type ENUM('Admin', 'Committee', 'Homestay', 'Activity', 'Event', 'Product'),
    IN p_entity_id INT
)
BEGIN
    SELECT 
        image_id,
        image_url,
        original_image_name,
        image_name,
        file_size,
        mime_type,
        entity_type, 
        entity_id,
        committee_id,
        uploaded_by,
        is_profile_image,
        status,
        created_at,
        updated_at
    FROM images 
    WHERE LOWER(entity_type) = LOWER(p_entity_type) 
      AND entity_id = p_entity_id
      AND status != 'Deleted'; -- Exclude deleted images
END ;;
DELIMITER ;
/*!50003 SET sql_mode              = @saved_sql_mode */ ;
/*!50003 SET character_set_client  = @saved_cs_client */ ;
/*!50003 SET character_set_results = @saved_cs_results */ ;
/*!50003 SET collation_connection  = @saved_col_connection */ ;
/*!50003 DROP PROCEDURE IF EXISTS `GetProductById` */;
/*!50003 SET @saved_cs_client      = @@character_set_client */ ;
/*!50003 SET @saved_cs_results     = @@character_set_results */ ;
/*!50003 SET @saved_col_connection = @@collation_connection */ ;
/*!50003 SET character_set_client  = utf8mb4 */ ;
/*!50003 SET character_set_results = utf8mb4 */ ;
/*!50003 SET collation_connection  = utf8mb4_0900_ai_ci */ ;
/*!50003 SET @saved_sql_mode       = @@sql_mode */ ;
/*!50003 SET sql_mode              = '' */ ;
DELIMITER ;;
CREATE DEFINER=`root`@`localhost` PROCEDURE `GetProductById`(IN p_product_id INT)
BEGIN
    SELECT 
        -- Product Details
        p.product_id, 
        p.product_name, 
        p.description, 
        p.metric_unit,
        p.metric_value,
        p.price, 
        
        -- Committee & District Details
        p.committee_id, 
        c.committee_name, 
        c.district_id, 
        d.district_name, 
        d.region, 
        
        -- Homestay Details (if applicable)
        p.homestay_id, 
        IFNULL(h.homestay_name, NULL) AS homestay_name,  -- Ensures NULL when homestay_id is NULL
        
        -- Verification & Status
        p.tags, 
        p.isVerifiable, 
        p.verification_status_id, 
        v.status_type,  -- Fetching status_type from master_verification_status
        p.is_active
        
    FROM products p
    -- Joining Committees to fetch committee_name & district_id
    JOIN committees c ON p.committee_id = c.committee_id
    -- Joining Districts to fetch district_name & region
    JOIN districts d ON c.district_id = d.district_id
    -- Joining Homestays to fetch homestay_name (if applicable)
    LEFT JOIN homestays h ON p.homestay_id = h.homestay_id
    -- Joining Verification Status to fetch status_type
    LEFT JOIN master_verification_status v ON p.verification_status_id = v.verification_status_id
    -- Filtering by the provided product ID
    WHERE p.product_id = p_product_id;
END ;;
DELIMITER ;
/*!50003 SET sql_mode              = @saved_sql_mode */ ;
/*!50003 SET character_set_client  = @saved_cs_client */ ;
/*!50003 SET character_set_results = @saved_cs_results */ ;
/*!50003 SET collation_connection  = @saved_col_connection */ ;
/*!50003 DROP PROCEDURE IF EXISTS `GetProfileImageByEntity` */;
/*!50003 SET @saved_cs_client      = @@character_set_client */ ;
/*!50003 SET @saved_cs_results     = @@character_set_results */ ;
/*!50003 SET @saved_col_connection = @@collation_connection */ ;
/*!50003 SET character_set_client  = utf8mb4 */ ;
/*!50003 SET character_set_results = utf8mb4 */ ;
/*!50003 SET collation_connection  = utf8mb4_0900_ai_ci */ ;
/*!50003 SET @saved_sql_mode       = @@sql_mode */ ;
/*!50003 SET sql_mode              = '' */ ;
DELIMITER ;;
CREATE DEFINER=`root`@`localhost` PROCEDURE `GetProfileImageByEntity`(
    IN p_entity_type ENUM('Admin', 'Committee', 'Homestay', 'Activity', 'Event', 'Product'),
    IN p_entity_id INT
)
BEGIN
    SELECT 
        image_id,
        image_url,
        original_image_name,
        image_name,
        file_size,
        mime_type,
        entity_type, 
        entity_id,
        committee_id,
        uploaded_by,
        is_profile_image,
        status,
        created_at,
        updated_at
    FROM images
    WHERE entity_type = p_entity_type 
      AND entity_id = p_entity_id
      AND is_profile_image = 1
    LIMIT 1;
END ;;
DELIMITER ;
/*!50003 SET sql_mode              = @saved_sql_mode */ ;
/*!50003 SET character_set_client  = @saved_cs_client */ ;
/*!50003 SET character_set_results = @saved_cs_results */ ;
/*!50003 SET collation_connection  = @saved_col_connection */ ;
/*!50003 DROP PROCEDURE IF EXISTS `GetUserByEmail` */;
/*!50003 SET @saved_cs_client      = @@character_set_client */ ;
/*!50003 SET @saved_cs_results     = @@character_set_results */ ;
/*!50003 SET @saved_col_connection = @@collation_connection */ ;
/*!50003 SET character_set_client  = utf8mb4 */ ;
/*!50003 SET character_set_results = utf8mb4 */ ;
/*!50003 SET collation_connection  = utf8mb4_0900_ai_ci */ ;
/*!50003 SET @saved_sql_mode       = @@sql_mode */ ;
/*!50003 SET sql_mode              = '' */ ;
DELIMITER ;;
CREATE DEFINER=`root`@`localhost` PROCEDURE `GetUserByEmail`(
    IN p_email VARCHAR(191)
    
    -- try@admin.com - admin123
)
BEGIN
    SELECT 
        u.user_id AS UserId,
        u.name AS Name,
        u.email AS Email,
        u.password AS Password,
        u.role_id AS RoleId,
        r.role_name AS RoleName,  -- If we need role name
        u.district_id AS DistrictId,
        u.is_active AS IsActive,
        u.created_at AS CreatedAt,
        u.updated_at AS UpdatedAt
    FROM users u
    LEFT JOIN user_roles r ON u.role_id = r.role_id
    WHERE u.email = p_email
    LIMIT 1;
END ;;
DELIMITER ;
/*!50003 SET sql_mode              = @saved_sql_mode */ ;
/*!50003 SET character_set_client  = @saved_cs_client */ ;
/*!50003 SET character_set_results = @saved_cs_results */ ;
/*!50003 SET collation_connection  = @saved_col_connection */ ;
/*!50003 DROP PROCEDURE IF EXISTS `GetUserById` */;
/*!50003 SET @saved_cs_client      = @@character_set_client */ ;
/*!50003 SET @saved_cs_results     = @@character_set_results */ ;
/*!50003 SET @saved_col_connection = @@collation_connection */ ;
/*!50003 SET character_set_client  = utf8mb4 */ ;
/*!50003 SET character_set_results = utf8mb4 */ ;
/*!50003 SET collation_connection  = utf8mb4_0900_ai_ci */ ;
/*!50003 SET @saved_sql_mode       = @@sql_mode */ ;
/*!50003 SET sql_mode              = '' */ ;
DELIMITER ;;
CREATE DEFINER=`root`@`localhost` PROCEDURE `GetUserById`(IN p_UserId INT)
BEGIN
    SELECT * FROM Users WHERE user_id = p_UserId;
END ;;
DELIMITER ;
/*!50003 SET sql_mode              = @saved_sql_mode */ ;
/*!50003 SET character_set_client  = @saved_cs_client */ ;
/*!50003 SET character_set_results = @saved_cs_results */ ;
/*!50003 SET collation_connection  = @saved_col_connection */ ;
/*!50003 DROP PROCEDURE IF EXISTS `InsertImage` */;
/*!50003 SET @saved_cs_client      = @@character_set_client */ ;
/*!50003 SET @saved_cs_results     = @@character_set_results */ ;
/*!50003 SET @saved_col_connection = @@collation_connection */ ;
/*!50003 SET character_set_client  = utf8mb4 */ ;
/*!50003 SET character_set_results = utf8mb4 */ ;
/*!50003 SET collation_connection  = utf8mb4_0900_ai_ci */ ;
/*!50003 SET @saved_sql_mode       = @@sql_mode */ ;
/*!50003 SET sql_mode              = '' */ ;
DELIMITER ;;
CREATE DEFINER=`root`@`localhost` PROCEDURE `InsertImage`(
    IN p_image_url VARCHAR(255),
    IN p_original_image_name VARCHAR(255),
    IN p_image_name VARCHAR(255),
    IN p_file_size BIGINT,
    IN p_mime_type VARCHAR(100),
    IN p_entity_type ENUM('Admin', 'Committee', 'Homestay', 'Activity', 'Event', 'Product'),
    IN p_entity_id INT,
    IN p_committee_id INT,  
    IN p_uploaded_by INT,
    IN p_is_profile_image TINYINT(1),
    IN p_status ENUM('Active', 'Archived', 'Deleted')
)
BEGIN
    -- Insert image details into the images table
    INSERT INTO images (
        image_url, 
        original_image_name, 
        image_name, 
        file_size, 
        mime_type, 
        entity_type, 
        entity_id, 
        committee_id, 
        uploaded_by, 
        is_profile_image, 
        status, 
        created_at, 
        updated_at
    ) 
    VALUES (
        p_image_url, 
        p_original_image_name, 
        p_image_name, 
        p_file_size, 
        p_mime_type, 
        p_entity_type, 
        p_entity_id, 
        IFNULL(p_committee_id, NULL),  -- Ensures NULL is stored if no committee ID is provided or Inserted by Admin
        IFNULL(p_uploaded_by, NULL),   -- Ensures NULL is stored if no uploader ID is provided
        p_is_profile_image, 
        p_status, 
        CURRENT_TIMESTAMP,  -- Sets the current timestamp for created_at
        CURRENT_TIMESTAMP   -- Sets the current timestamp for updated_at
    );
END ;;
DELIMITER ;
/*!50003 SET sql_mode              = @saved_sql_mode */ ;
/*!50003 SET character_set_client  = @saved_cs_client */ ;
/*!50003 SET character_set_results = @saved_cs_results */ ;
/*!50003 SET collation_connection  = @saved_col_connection */ ;
/*!50003 DROP PROCEDURE IF EXISTS `InsertRefreshToken` */;
/*!50003 SET @saved_cs_client      = @@character_set_client */ ;
/*!50003 SET @saved_cs_results     = @@character_set_results */ ;
/*!50003 SET @saved_col_connection = @@collation_connection */ ;
/*!50003 SET character_set_client  = utf8mb4 */ ;
/*!50003 SET character_set_results = utf8mb4 */ ;
/*!50003 SET collation_connection  = utf8mb4_0900_ai_ci */ ;
/*!50003 SET @saved_sql_mode       = @@sql_mode */ ;
/*!50003 SET sql_mode              = '' */ ;
DELIMITER ;;
CREATE DEFINER=`root`@`localhost` PROCEDURE `InsertRefreshToken`(
IN p_user_id INT, 
IN p_refresh_token VARCHAR(255), 
IN p_expires_at DATETIME)
BEGIN
    INSERT INTO refreshtokens (UserId, Token, ExpiresAt, CreatedAt, IsRevoked) 
    VALUES (p_user_id, p_refresh_token, p_expires_at, NOW(), 0);
END ;;
DELIMITER ;
/*!50003 SET sql_mode              = @saved_sql_mode */ ;
/*!50003 SET character_set_client  = @saved_cs_client */ ;
/*!50003 SET character_set_results = @saved_cs_results */ ;
/*!50003 SET collation_connection  = @saved_col_connection */ ;
/*!50003 DROP PROCEDURE IF EXISTS `RegisterUser` */;
/*!50003 SET @saved_cs_client      = @@character_set_client */ ;
/*!50003 SET @saved_cs_results     = @@character_set_results */ ;
/*!50003 SET @saved_col_connection = @@collation_connection */ ;
/*!50003 SET character_set_client  = utf8mb4 */ ;
/*!50003 SET character_set_results = utf8mb4 */ ;
/*!50003 SET collation_connection  = utf8mb4_0900_ai_ci */ ;
/*!50003 SET @saved_sql_mode       = @@sql_mode */ ;
/*!50003 SET sql_mode              = '' */ ;
DELIMITER ;;
CREATE DEFINER=`root`@`localhost` PROCEDURE `RegisterUser`(
    IN p_name VARCHAR(255),
    IN p_email VARCHAR(191),
    IN p_password VARCHAR(255),
    IN p_role_id INT,
    IN p_district_id INT
)
BEGIN
    -- Check if the email already exists
    IF EXISTS (SELECT 1 FROM users WHERE email = p_email) THEN
        SIGNAL SQLSTATE '45000' SET MESSAGE_TEXT = 'User already exists with this email.';
    ELSE
        -- Insert new user
        INSERT INTO users (name, email, password, role_id, district_id)
        VALUES (
            p_name,
            p_email,
            p_password,
            p_role_id,
            IF(p_role_id = 1, NULL, p_district_id) -- If role_name = 'Admin' no need for district_id
        );
    END IF;
END ;;
DELIMITER ;
/*!50003 SET sql_mode              = @saved_sql_mode */ ;
/*!50003 SET character_set_client  = @saved_cs_client */ ;
/*!50003 SET character_set_results = @saved_cs_results */ ;
/*!50003 SET collation_connection  = @saved_col_connection */ ;
/*!50003 DROP PROCEDURE IF EXISTS `RevokeAllRefreshTokens` */;
/*!50003 SET @saved_cs_client      = @@character_set_client */ ;
/*!50003 SET @saved_cs_results     = @@character_set_results */ ;
/*!50003 SET @saved_col_connection = @@collation_connection */ ;
/*!50003 SET character_set_client  = utf8mb4 */ ;
/*!50003 SET character_set_results = utf8mb4 */ ;
/*!50003 SET collation_connection  = utf8mb4_0900_ai_ci */ ;
/*!50003 SET @saved_sql_mode       = @@sql_mode */ ;
/*!50003 SET sql_mode              = '' */ ;
DELIMITER ;;
CREATE DEFINER=`root`@`localhost` PROCEDURE `RevokeAllRefreshTokens`(
    IN p_user_id INT
)
BEGIN
    UPDATE refreshtokens
    SET IsRevoked = 1, RevokedAt = NOW()
    WHERE UserId = p_user_id;
END ;;
DELIMITER ;
/*!50003 SET sql_mode              = @saved_sql_mode */ ;
/*!50003 SET character_set_client  = @saved_cs_client */ ;
/*!50003 SET character_set_results = @saved_cs_results */ ;
/*!50003 SET collation_connection  = @saved_col_connection */ ;
/*!50003 DROP PROCEDURE IF EXISTS `RevokeRefreshToken` */;
/*!50003 SET @saved_cs_client      = @@character_set_client */ ;
/*!50003 SET @saved_cs_results     = @@character_set_results */ ;
/*!50003 SET @saved_col_connection = @@collation_connection */ ;
/*!50003 SET character_set_client  = utf8mb4 */ ;
/*!50003 SET character_set_results = utf8mb4 */ ;
/*!50003 SET collation_connection  = utf8mb4_0900_ai_ci */ ;
/*!50003 SET @saved_sql_mode       = @@sql_mode */ ;
/*!50003 SET sql_mode              = '' */ ;
DELIMITER ;;
CREATE DEFINER=`root`@`localhost` PROCEDURE `RevokeRefreshToken`(
    IN p_user_id INT,
    IN p_refresh_token VARCHAR(255)
)
BEGIN
    UPDATE refreshtokens
    SET IsRevoked = 1, RevokedAt = NOW()
    WHERE UserId = p_user_id 
    AND Token = p_refresh_token;
END ;;
DELIMITER ;
/*!50003 SET sql_mode              = @saved_sql_mode */ ;
/*!50003 SET character_set_client  = @saved_cs_client */ ;
/*!50003 SET character_set_results = @saved_cs_results */ ;
/*!50003 SET collation_connection  = @saved_col_connection */ ;
/*!50003 DROP PROCEDURE IF EXISTS `SetProfileImage` */;
/*!50003 SET @saved_cs_client      = @@character_set_client */ ;
/*!50003 SET @saved_cs_results     = @@character_set_results */ ;
/*!50003 SET @saved_col_connection = @@collation_connection */ ;
/*!50003 SET character_set_client  = utf8mb4 */ ;
/*!50003 SET character_set_results = utf8mb4 */ ;
/*!50003 SET collation_connection  = utf8mb4_0900_ai_ci */ ;
/*!50003 SET @saved_sql_mode       = @@sql_mode */ ;
/*!50003 SET sql_mode              = '' */ ;
DELIMITER ;;
CREATE DEFINER=`root`@`localhost` PROCEDURE `SetProfileImage`(
    IN p_image_id INT
)
BEGIN
    DECLARE v_entity_type ENUM('Admin', 'Committee', 'Homestay', 'Activity', 'Event', 'Product');
    DECLARE v_entity_id INT;
    DECLARE v_committee_id INT;

    -- Get entity details of the image being set as profile
    SELECT entity_type, entity_id, committee_id 
    INTO v_entity_type, v_entity_id, v_committee_id
    FROM images 
    WHERE image_id = p_image_id;

    -- Ensure the image exists
    IF v_entity_id IS NOT NULL THEN
        -- Unset previous profile image for the same entity_id and committee_id
        UPDATE images 
        SET is_profile_image = 0 
        WHERE entity_type = v_entity_type 
          AND entity_id = v_entity_id 
          AND committee_id = v_committee_id;

        -- Set the selected image as the new profile image
        UPDATE images 
        SET is_profile_image = 1 
        WHERE image_id = p_image_id;
    END IF;
END ;;
DELIMITER ;
/*!50003 SET sql_mode              = @saved_sql_mode */ ;
/*!50003 SET character_set_client  = @saved_cs_client */ ;
/*!50003 SET character_set_results = @saved_cs_results */ ;
/*!50003 SET collation_connection  = @saved_col_connection */ ;
/*!50003 DROP PROCEDURE IF EXISTS `ToggleActivityStatus` */;
/*!50003 SET @saved_cs_client      = @@character_set_client */ ;
/*!50003 SET @saved_cs_results     = @@character_set_results */ ;
/*!50003 SET @saved_col_connection = @@collation_connection */ ;
/*!50003 SET character_set_client  = utf8mb4 */ ;
/*!50003 SET character_set_results = utf8mb4 */ ;
/*!50003 SET collation_connection  = utf8mb4_0900_ai_ci */ ;
/*!50003 SET @saved_sql_mode       = @@sql_mode */ ;
/*!50003 SET sql_mode              = '' */ ;
DELIMITER ;;
CREATE DEFINER=`root`@`localhost` PROCEDURE `ToggleActivityStatus`(
    IN p_activity_id INT,
    IN p_is_active TINYINT(1)
)
BEGIN
    DECLARE rows_affected INT;

    -- Check if the activity exists before updating
    IF EXISTS (SELECT 1 FROM activities WHERE activity_id = p_activity_id) THEN
        UPDATE activities
        SET is_active = p_is_active
        WHERE activity_id = p_activity_id;
        
        SET rows_affected = ROW_COUNT();
    ELSE
        SET rows_affected = 0; -- Activity does not exist
    END IF;

    -- ✅ Return 1 if update was successful, 0 otherwise
    SELECT rows_affected AS success;
END ;;
DELIMITER ;
/*!50003 SET sql_mode              = @saved_sql_mode */ ;
/*!50003 SET character_set_client  = @saved_cs_client */ ;
/*!50003 SET character_set_results = @saved_cs_results */ ;
/*!50003 SET collation_connection  = @saved_col_connection */ ;
/*!50003 DROP PROCEDURE IF EXISTS `ToggleCommitteeStatus` */;
/*!50003 SET @saved_cs_client      = @@character_set_client */ ;
/*!50003 SET @saved_cs_results     = @@character_set_results */ ;
/*!50003 SET @saved_col_connection = @@collation_connection */ ;
/*!50003 SET character_set_client  = utf8mb4 */ ;
/*!50003 SET character_set_results = utf8mb4 */ ;
/*!50003 SET collation_connection  = utf8mb4_0900_ai_ci */ ;
/*!50003 SET @saved_sql_mode       = @@sql_mode */ ;
/*!50003 SET sql_mode              = '' */ ;
DELIMITER ;;
CREATE DEFINER=`root`@`localhost` PROCEDURE `ToggleCommitteeStatus`(IN p_committee_id INT, IN p_is_active TINYINT)
BEGIN
    UPDATE committees 
    SET is_active = p_is_active 
    WHERE committee_id = p_committee_id;
    
    -- Return success status
    SELECT ROW_COUNT() AS success;
END ;;
DELIMITER ;
/*!50003 SET sql_mode              = @saved_sql_mode */ ;
/*!50003 SET character_set_client  = @saved_cs_client */ ;
/*!50003 SET character_set_results = @saved_cs_results */ ;
/*!50003 SET collation_connection  = @saved_col_connection */ ;
/*!50003 DROP PROCEDURE IF EXISTS `ToggleEventStatus` */;
/*!50003 SET @saved_cs_client      = @@character_set_client */ ;
/*!50003 SET @saved_cs_results     = @@character_set_results */ ;
/*!50003 SET @saved_col_connection = @@collation_connection */ ;
/*!50003 SET character_set_client  = utf8mb4 */ ;
/*!50003 SET character_set_results = utf8mb4 */ ;
/*!50003 SET collation_connection  = utf8mb4_0900_ai_ci */ ;
/*!50003 SET @saved_sql_mode       = @@sql_mode */ ;
/*!50003 SET sql_mode              = '' */ ;
DELIMITER ;;
CREATE DEFINER=`root`@`localhost` PROCEDURE `ToggleEventStatus`(
    IN p_event_id INT,
    IN p_is_active TINYINT
)
BEGIN
    UPDATE events 
    SET is_active = p_is_active 
    WHERE event_id = p_event_id;

    -- Return success flag
    SELECT ROW_COUNT() AS success;
END ;;
DELIMITER ;
/*!50003 SET sql_mode              = @saved_sql_mode */ ;
/*!50003 SET character_set_client  = @saved_cs_client */ ;
/*!50003 SET character_set_results = @saved_cs_results */ ;
/*!50003 SET collation_connection  = @saved_col_connection */ ;
/*!50003 DROP PROCEDURE IF EXISTS `ToggleHomestayStatus` */;
/*!50003 SET @saved_cs_client      = @@character_set_client */ ;
/*!50003 SET @saved_cs_results     = @@character_set_results */ ;
/*!50003 SET @saved_col_connection = @@collation_connection */ ;
/*!50003 SET character_set_client  = utf8mb4 */ ;
/*!50003 SET character_set_results = utf8mb4 */ ;
/*!50003 SET collation_connection  = utf8mb4_0900_ai_ci */ ;
/*!50003 SET @saved_sql_mode       = @@sql_mode */ ;
/*!50003 SET sql_mode              = '' */ ;
DELIMITER ;;
CREATE DEFINER=`root`@`localhost` PROCEDURE `ToggleHomestayStatus`(
    IN p_homestay_id INT,
    IN p_is_active TINYINT
)
BEGIN
    UPDATE homestays 
    SET is_active = p_is_active 
    WHERE homestay_id = p_homestay_id;
END ;;
DELIMITER ;
/*!50003 SET sql_mode              = @saved_sql_mode */ ;
/*!50003 SET character_set_client  = @saved_cs_client */ ;
/*!50003 SET character_set_results = @saved_cs_results */ ;
/*!50003 SET collation_connection  = @saved_col_connection */ ;
/*!50003 DROP PROCEDURE IF EXISTS `ToggleProductStatus` */;
/*!50003 SET @saved_cs_client      = @@character_set_client */ ;
/*!50003 SET @saved_cs_results     = @@character_set_results */ ;
/*!50003 SET @saved_col_connection = @@collation_connection */ ;
/*!50003 SET character_set_client  = utf8mb4 */ ;
/*!50003 SET character_set_results = utf8mb4 */ ;
/*!50003 SET collation_connection  = utf8mb4_0900_ai_ci */ ;
/*!50003 SET @saved_sql_mode       = @@sql_mode */ ;
/*!50003 SET sql_mode              = '' */ ;
DELIMITER ;;
CREATE DEFINER=`root`@`localhost` PROCEDURE `ToggleProductStatus`(
    IN p_product_id INT,
    IN p_is_active BOOLEAN
)
BEGIN
    UPDATE products 
    SET is_active = p_is_active
    WHERE product_id = p_product_id;
END ;;
DELIMITER ;
/*!50003 SET sql_mode              = @saved_sql_mode */ ;
/*!50003 SET character_set_client  = @saved_cs_client */ ;
/*!50003 SET character_set_results = @saved_cs_results */ ;
/*!50003 SET collation_connection  = @saved_col_connection */ ;
/*!50003 DROP PROCEDURE IF EXISTS `UnarchiveUser` */;
/*!50003 SET @saved_cs_client      = @@character_set_client */ ;
/*!50003 SET @saved_cs_results     = @@character_set_results */ ;
/*!50003 SET @saved_col_connection = @@collation_connection */ ;
/*!50003 SET character_set_client  = utf8mb4 */ ;
/*!50003 SET character_set_results = utf8mb4 */ ;
/*!50003 SET collation_connection  = utf8mb4_0900_ai_ci */ ;
/*!50003 SET @saved_sql_mode       = @@sql_mode */ ;
/*!50003 SET sql_mode              = '' */ ;
DELIMITER ;;
CREATE DEFINER=`root`@`localhost` PROCEDURE `UnarchiveUser`(IN p_UserId INT)
BEGIN
    UPDATE Users SET is_active = 1 WHERE user_id = p_UserId;
END ;;
DELIMITER ;
/*!50003 SET sql_mode              = @saved_sql_mode */ ;
/*!50003 SET character_set_client  = @saved_cs_client */ ;
/*!50003 SET character_set_results = @saved_cs_results */ ;
/*!50003 SET collation_connection  = @saved_col_connection */ ;
/*!50003 DROP PROCEDURE IF EXISTS `UpdateActivity` */;
/*!50003 SET @saved_cs_client      = @@character_set_client */ ;
/*!50003 SET @saved_cs_results     = @@character_set_results */ ;
/*!50003 SET @saved_col_connection = @@collation_connection */ ;
/*!50003 SET character_set_client  = utf8mb4 */ ;
/*!50003 SET character_set_results = utf8mb4 */ ;
/*!50003 SET collation_connection  = utf8mb4_0900_ai_ci */ ;
/*!50003 SET @saved_sql_mode       = @@sql_mode */ ;
/*!50003 SET sql_mode              = '' */ ;
DELIMITER ;;
CREATE DEFINER=`root`@`localhost` PROCEDURE `UpdateActivity`(
    IN p_activity_id INT,
    IN p_activity_name VARCHAR(255),
    IN p_description TEXT,
    IN p_tags JSON,
    IN p_committee_id INT,
    IN p_homestay_id INT,
    IN p_isVerifiable BOOLEAN,
    IN p_verification_status_id INT,
    IN p_is_active BOOLEAN
)
BEGIN
    DECLARE rows_affected INT;

    UPDATE activities
    SET 
        activity_name = COALESCE(NULLIF(p_activity_name, ''), activity_name),
        description = COALESCE(NULLIF(p_description, ''), description),
        tags = COALESCE(NULLIF(p_tags, '[]'), tags),  -- ✅ Convert empty JSON array to NULL
        committee_id = p_committee_id,
        homestay_id = NULLIF(p_homestay_id, 0),  -- ✅ Convert 0 to NULL
        isVerifiable = p_isVerifiable,
        verification_status_id = p_verification_status_id,
        is_active = p_is_active
    WHERE activity_id = p_activity_id;

    SET rows_affected = ROW_COUNT();

    -- ✅ Return 1 if update was successful, 0 otherwise
    SELECT rows_affected AS success;
END ;;
DELIMITER ;
/*!50003 SET sql_mode              = @saved_sql_mode */ ;
/*!50003 SET character_set_client  = @saved_cs_client */ ;
/*!50003 SET character_set_results = @saved_cs_results */ ;
/*!50003 SET collation_connection  = @saved_col_connection */ ;
/*!50003 DROP PROCEDURE IF EXISTS `UpdateCommittee` */;
/*!50003 SET @saved_cs_client      = @@character_set_client */ ;
/*!50003 SET @saved_cs_results     = @@character_set_results */ ;
/*!50003 SET @saved_col_connection = @@collation_connection */ ;
/*!50003 SET character_set_client  = utf8mb4 */ ;
/*!50003 SET character_set_results = utf8mb4 */ ;
/*!50003 SET collation_connection  = utf8mb4_0900_ai_ci */ ;
/*!50003 SET @saved_sql_mode       = @@sql_mode */ ;
/*!50003 SET sql_mode              = '' */ ;
DELIMITER ;;
CREATE DEFINER=`root`@`localhost` PROCEDURE `UpdateCommittee`(
    IN p_committee_id INT,
    IN p_committee_name VARCHAR(255),
    IN p_description TEXT,
    IN p_district_id INT,
    IN p_contact_number VARCHAR(20),
    IN p_email VARCHAR(191),
    IN p_address TEXT,
    IN p_tags JSON,
    IN p_tourist_attractions JSON,
    IN p_latitude DECIMAL(10,6),
    IN p_longitude DECIMAL(10,6),
    IN p_leadership JSON,
    IN p_isVerifiable TINYINT,
    IN p_verification_status_id INT,
    IN p_is_active TINYINT
)
BEGIN
    UPDATE committees
    SET committee_name = p_committee_name,
        description = p_description,
        district_id = p_district_id,
        contact_number = p_contact_number,
        email = p_email,
        address = p_address,
        tags = p_tags,
        tourist_attractions = p_tourist_attractions,
        latitude = p_latitude,
        longitude = p_longitude,
        leadership = p_leadership,
        isVerifiable = p_isVerifiable,
        verification_status_id = p_verification_status_id,
        is_active = p_is_active
    WHERE committee_id = p_committee_id;
END ;;
DELIMITER ;
/*!50003 SET sql_mode              = @saved_sql_mode */ ;
/*!50003 SET character_set_client  = @saved_cs_client */ ;
/*!50003 SET character_set_results = @saved_cs_results */ ;
/*!50003 SET collation_connection  = @saved_col_connection */ ;
/*!50003 DROP PROCEDURE IF EXISTS `UpdateDistrict` */;
/*!50003 SET @saved_cs_client      = @@character_set_client */ ;
/*!50003 SET @saved_cs_results     = @@character_set_results */ ;
/*!50003 SET @saved_col_connection = @@collation_connection */ ;
/*!50003 SET character_set_client  = utf8mb4 */ ;
/*!50003 SET character_set_results = utf8mb4 */ ;
/*!50003 SET collation_connection  = utf8mb4_0900_ai_ci */ ;
/*!50003 SET @saved_sql_mode       = @@sql_mode */ ;
/*!50003 SET sql_mode              = '' */ ;
DELIMITER ;;
CREATE DEFINER=`root`@`localhost` PROCEDURE `UpdateDistrict`(
    IN districtId INT,
    IN districtName VARCHAR(191),
    IN region ENUM('North','East','West','South')
)
BEGIN
    UPDATE districts 
    SET district_name = districtName, region = region 
    WHERE district_id = districtId;
END ;;
DELIMITER ;
/*!50003 SET sql_mode              = @saved_sql_mode */ ;
/*!50003 SET character_set_client  = @saved_cs_client */ ;
/*!50003 SET character_set_results = @saved_cs_results */ ;
/*!50003 SET collation_connection  = @saved_col_connection */ ;
/*!50003 DROP PROCEDURE IF EXISTS `UpdateEvent` */;
/*!50003 SET @saved_cs_client      = @@character_set_client */ ;
/*!50003 SET @saved_cs_results     = @@character_set_results */ ;
/*!50003 SET @saved_col_connection = @@collation_connection */ ;
/*!50003 SET character_set_client  = utf8mb4 */ ;
/*!50003 SET character_set_results = utf8mb4 */ ;
/*!50003 SET collation_connection  = utf8mb4_0900_ai_ci */ ;
/*!50003 SET @saved_sql_mode       = @@sql_mode */ ;
/*!50003 SET sql_mode              = '' */ ;
DELIMITER ;;
CREATE DEFINER=`root`@`localhost` PROCEDURE `UpdateEvent`(
    IN p_event_id INT,
    IN p_event_name VARCHAR(255),
    IN p_description TEXT,
    IN p_start_date DATE,
    IN p_end_date DATE,
    IN p_location VARCHAR(255),
    IN p_committee_id INT,
    IN p_tags JSON,
    IN p_isVerifiable TINYINT,
    IN p_verification_status_id INT,
    IN p_is_active TINYINT
)
BEGIN
    UPDATE events 
    SET event_name = p_event_name, 
        description = p_description,
        start_date = p_start_date, 
        end_date = p_end_date, 
        location = p_location, 
        committee_id = p_committee_id, 
        tags = p_tags, 
        isVerifiable = p_isVerifiable, 
        verification_status_id = p_verification_status_id, 
        is_active = p_is_active
    WHERE event_id = p_event_id;
END ;;
DELIMITER ;
/*!50003 SET sql_mode              = @saved_sql_mode */ ;
/*!50003 SET character_set_client  = @saved_cs_client */ ;
/*!50003 SET character_set_results = @saved_cs_results */ ;
/*!50003 SET collation_connection  = @saved_col_connection */ ;
/*!50003 DROP PROCEDURE IF EXISTS `UpdateHomestay` */;
/*!50003 SET @saved_cs_client      = @@character_set_client */ ;
/*!50003 SET @saved_cs_results     = @@character_set_results */ ;
/*!50003 SET @saved_col_connection = @@collation_connection */ ;
/*!50003 SET character_set_client  = utf8mb4 */ ;
/*!50003 SET character_set_results = utf8mb4 */ ;
/*!50003 SET collation_connection  = utf8mb4_0900_ai_ci */ ;
/*!50003 SET @saved_sql_mode       = @@sql_mode */ ;
/*!50003 SET sql_mode              = '' */ ;
DELIMITER ;;
CREATE DEFINER=`root`@`localhost` PROCEDURE `UpdateHomestay`(
    IN p_homestay_id INT,
    IN p_homestay_name VARCHAR(255),
    IN p_committee_id INT,
    IN p_address TEXT,
    IN p_description TEXT,
    IN p_owner_name VARCHAR(255),
    IN p_owner_mobile VARCHAR(20),
    IN p_total_rooms INT,
    IN p_room_tariff DECIMAL(10,2),
    IN p_tags JSON,
    IN p_amenities JSON,
    IN p_payment_methods VARCHAR(255),
    IN p_social_media_links JSON,
    IN p_isVerifiable TINYINT,
    IN p_verification_status_id INT,
    IN p_is_active TINYINT
)
BEGIN
    UPDATE homestays 
    SET 
        homestay_name = p_homestay_name,
        committee_id = p_committee_id,
        address = p_address,
        description = p_description,
        owner_name = p_owner_name,
        owner_mobile = p_owner_mobile,
        total_rooms = p_total_rooms,
        room_tariff = p_room_tariff,
        tags = p_tags,
        amenities = p_amenities,
        payment_methods = p_payment_methods,
        social_media_links = p_social_media_links,
        isVerifiable = p_isVerifiable,
        verification_status_id = p_verification_status_id,
        is_active = p_is_active
    WHERE homestay_id = p_homestay_id;
END ;;
DELIMITER ;
/*!50003 SET sql_mode              = @saved_sql_mode */ ;
/*!50003 SET character_set_client  = @saved_cs_client */ ;
/*!50003 SET character_set_results = @saved_cs_results */ ;
/*!50003 SET collation_connection  = @saved_col_connection */ ;
/*!50003 DROP PROCEDURE IF EXISTS `UpdateImage` */;
/*!50003 SET @saved_cs_client      = @@character_set_client */ ;
/*!50003 SET @saved_cs_results     = @@character_set_results */ ;
/*!50003 SET @saved_col_connection = @@collation_connection */ ;
/*!50003 SET character_set_client  = utf8mb4 */ ;
/*!50003 SET character_set_results = utf8mb4 */ ;
/*!50003 SET collation_connection  = utf8mb4_0900_ai_ci */ ;
/*!50003 SET @saved_sql_mode       = @@sql_mode */ ;
/*!50003 SET sql_mode              = '' */ ;
DELIMITER ;;
CREATE DEFINER=`root`@`localhost` PROCEDURE `UpdateImage`(
    IN p_image_id INT,
    IN p_new_image_url VARCHAR(255),
    IN p_original_image_name VARCHAR(255),
    IN p_image_name VARCHAR(255),
    IN p_file_size BIGINT,
    IN p_mime_type VARCHAR(100)
    -- IN p_is_profile_image TINYINT(1)
)
BEGIN
    -- Check if image exists before updating
    IF EXISTS (SELECT 1 FROM images WHERE image_id = p_image_id) THEN
        UPDATE images
        SET 
            image_url = IFNULL(p_new_image_url, image_url),
            original_image_name = IFNULL(p_original_image_name, original_image_name),
            image_name = IFNULL(p_image_name, image_name),
            file_size = IFNULL(p_file_size, file_size),
            mime_type = IFNULL(p_mime_type, mime_type),
            updated_at = CURRENT_TIMESTAMP
        WHERE image_id = p_image_id;
    ELSE
        SIGNAL SQLSTATE '45000' 
        SET MESSAGE_TEXT = 'Image ID not found';
    END IF;
END ;;
DELIMITER ;
/*!50003 SET sql_mode              = @saved_sql_mode */ ;
/*!50003 SET character_set_client  = @saved_cs_client */ ;
/*!50003 SET character_set_results = @saved_cs_results */ ;
/*!50003 SET collation_connection  = @saved_col_connection */ ;
/*!50003 DROP PROCEDURE IF EXISTS `UpdateProduct` */;
/*!50003 SET @saved_cs_client      = @@character_set_client */ ;
/*!50003 SET @saved_cs_results     = @@character_set_results */ ;
/*!50003 SET @saved_col_connection = @@collation_connection */ ;
/*!50003 SET character_set_client  = utf8mb4 */ ;
/*!50003 SET character_set_results = utf8mb4 */ ;
/*!50003 SET collation_connection  = utf8mb4_0900_ai_ci */ ;
/*!50003 SET @saved_sql_mode       = @@sql_mode */ ;
/*!50003 SET sql_mode              = '' */ ;
DELIMITER ;;
CREATE DEFINER=`root`@`localhost` PROCEDURE `UpdateProduct`(
    IN p_product_id INT,
    IN p_product_name VARCHAR(255),
    IN p_description TEXT,
    IN p_metric_unit VARCHAR(50),
    IN p_metric_value DECIMAL(10,2),
    IN p_price DECIMAL(10,2),
    IN p_committee_id INT,
    IN p_homestay_id INT,
    IN p_tags JSON,
    IN p_isVerifiable BOOLEAN,
    IN p_verification_status_id INT,
    IN p_is_active BOOLEAN
)
BEGIN
    UPDATE products
    SET 
        product_name = p_product_name,
        description = p_description,
        metric_unit = IFNULL(p_metric_unit, 'Per Unit'), -- Default value if NULL
        metric_value = IFNULL(p_metric_value, NULL),
        price = p_price,
        committee_id = p_committee_id,
        homestay_id = IFNULL(p_homestay_id, NULL),
        tags = IFNULL(p_tags, NULL),
        isVerifiable = p_isVerifiable,
        verification_status_id = p_verification_status_id,
        is_active = p_is_active
    WHERE product_id = p_product_id;
END ;;
DELIMITER ;
/*!50003 SET sql_mode              = @saved_sql_mode */ ;
/*!50003 SET character_set_client  = @saved_cs_client */ ;
/*!50003 SET character_set_results = @saved_cs_results */ ;
/*!50003 SET collation_connection  = @saved_col_connection */ ;
/*!50003 DROP PROCEDURE IF EXISTS `UpdateRefreshToken` */;
/*!50003 SET @saved_cs_client      = @@character_set_client */ ;
/*!50003 SET @saved_cs_results     = @@character_set_results */ ;
/*!50003 SET @saved_col_connection = @@collation_connection */ ;
/*!50003 SET character_set_client  = utf8mb4 */ ;
/*!50003 SET character_set_results = utf8mb4 */ ;
/*!50003 SET collation_connection  = utf8mb4_0900_ai_ci */ ;
/*!50003 SET @saved_sql_mode       = @@sql_mode */ ;
/*!50003 SET sql_mode              = '' */ ;
DELIMITER ;;
CREATE DEFINER=`root`@`localhost` PROCEDURE `UpdateRefreshToken`(
    IN p_user_id INT,
    IN p_refresh_token VARCHAR(255),
    IN p_expires_at DATETIME
)
BEGIN
     -- Insert new refresh token instead of updating
    -- Ensure refreshtokens table allows multiple refresh tokens per user.
    INSERT INTO refreshtokens (UserId, Token, ExpiresAt, CreatedAt, IsRevoked, RevokedAt)
    VALUES (p_user_id, p_refresh_token, p_expires_at, NOW(), 0, NULL);
END ;;
DELIMITER ;
/*!50003 SET sql_mode              = @saved_sql_mode */ ;
/*!50003 SET character_set_client  = @saved_cs_client */ ;
/*!50003 SET character_set_results = @saved_cs_results */ ;
/*!50003 SET collation_connection  = @saved_col_connection */ ;
/*!50003 DROP PROCEDURE IF EXISTS `UpdateUser` */;
/*!50003 SET @saved_cs_client      = @@character_set_client */ ;
/*!50003 SET @saved_cs_results     = @@character_set_results */ ;
/*!50003 SET @saved_col_connection = @@collation_connection */ ;
/*!50003 SET character_set_client  = utf8mb4 */ ;
/*!50003 SET character_set_results = utf8mb4 */ ;
/*!50003 SET collation_connection  = utf8mb4_0900_ai_ci */ ;
/*!50003 SET @saved_sql_mode       = @@sql_mode */ ;
/*!50003 SET sql_mode              = '' */ ;
DELIMITER ;;
CREATE DEFINER=`root`@`localhost` PROCEDURE `UpdateUser`(
    IN p_UserId INT,
    IN p_Name VARCHAR(255),
    IN p_Email VARCHAR(255),
    IN p_Password VARCHAR(255),
    IN p_CommitteeId INT,
    IN p_IsActive BOOLEAN
)
BEGIN
    UPDATE Users 
    SET name = p_Name, email = p_Email, password = p_Password, 
        committee_id = p_CommitteeId, is_active = p_IsActive
    WHERE user_id = p_UserId;
END ;;
DELIMITER ;
/*!50003 SET sql_mode              = @saved_sql_mode */ ;
/*!50003 SET character_set_client  = @saved_cs_client */ ;
/*!50003 SET character_set_results = @saved_cs_results */ ;
/*!50003 SET collation_connection  = @saved_col_connection */ ;
/*!50003 DROP PROCEDURE IF EXISTS `ValidateRefreshToken` */;
/*!50003 SET @saved_cs_client      = @@character_set_client */ ;
/*!50003 SET @saved_cs_results     = @@character_set_results */ ;
/*!50003 SET @saved_col_connection = @@collation_connection */ ;
/*!50003 SET character_set_client  = utf8mb4 */ ;
/*!50003 SET character_set_results = utf8mb4 */ ;
/*!50003 SET collation_connection  = utf8mb4_0900_ai_ci */ ;
/*!50003 SET @saved_sql_mode       = @@sql_mode */ ;
/*!50003 SET sql_mode              = '' */ ;
DELIMITER ;;
CREATE DEFINER=`root`@`localhost` PROCEDURE `ValidateRefreshToken`(
    IN p_user_id INT,
    IN p_refresh_token VARCHAR(255)
)
BEGIN
    SELECT Id, UserId, Token, ExpiresAt, CreatedAt, IsRevoked, RevokedAt
    FROM refreshtokens
    WHERE UserId = p_user_id 
    AND Token = p_refresh_token
    AND ExpiresAt > NOW()  -- Ensures token is not expired
    AND IsRevoked = 0;  -- Ensures token is not revoked
END ;;
DELIMITER ;
/*!50003 SET sql_mode              = @saved_sql_mode */ ;
/*!50003 SET character_set_client  = @saved_cs_client */ ;
/*!50003 SET character_set_results = @saved_cs_results */ ;
/*!50003 SET collation_connection  = @saved_col_connection */ ;
/*!40103 SET TIME_ZONE=@OLD_TIME_ZONE */;

/*!40101 SET SQL_MODE=@OLD_SQL_MODE */;
/*!40014 SET FOREIGN_KEY_CHECKS=@OLD_FOREIGN_KEY_CHECKS */;
/*!40014 SET UNIQUE_CHECKS=@OLD_UNIQUE_CHECKS */;
/*!40101 SET CHARACTER_SET_CLIENT=@OLD_CHARACTER_SET_CLIENT */;
/*!40101 SET CHARACTER_SET_RESULTS=@OLD_CHARACTER_SET_RESULTS */;
/*!40101 SET COLLATION_CONNECTION=@OLD_COLLATION_CONNECTION */;
/*!40111 SET SQL_NOTES=@OLD_SQL_NOTES */;

-- Dump completed on 2025-03-15 10:16:21

using ECommerceAPI.Data;
using ECommerceAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace ECommerceAPI.Data;

public static class DbDefaultProducts // Static porque solo se necesita instanciar una vez.
{
    public static async Task SeedAsync(AppDbContext context)
    {
        // Default Admin User.
        if (!await context.Users.AnyAsync(u => u.Rol == "Admin"))
        {
            var adminEmail = "admin@ecommerce.com";
            var adminPassword = "Admin123"; // TODO: Ponerlo en variables de entorno.

            var admin = new User
            {
                FullName = "Admin",
                Email = adminEmail,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(adminPassword),
                Rol = "Admin"
            };

            context.Users.Add(admin);
            await context.SaveChangesAsync();
        }

        // Categorias por defecto.
        var defaultNames = new[] { "Auriculares", "Consolas", "Celulares", "Graficas" };
        foreach (var name in defaultNames)
        {
            var exists = await context.Categories.AnyAsync(c => c.Name == name);
            if (!exists)
                context.Categories.Add(new Category { Name = name });
        }
        await context.SaveChangesAsync();

        var auricularesId = await context.Categories.Where(c => c.Name == "Auriculares").Select(c => c.Id).FirstOrDefaultAsync();
        var consolasId = await context.Categories.Where(c => c.Name == "Consolas").Select(c => c.Id).FirstOrDefaultAsync();
        var celularesId = await context.Categories.Where(c => c.Name == "Celulares").Select(c => c.Id).FirstOrDefaultAsync();
        var graficasId = await context.Categories.Where(c => c.Name == "Graficas").Select(c => c.Id).FirstOrDefaultAsync();

        if (auricularesId == 0 || consolasId == 0 || consolasId == 0 || graficasId == 0)
            throw new Exception("No se pudieron crear/obtener las categorías por defecto..");

        // Productos por default.
        if (!await context.Products.AnyAsync())
        {
            var products = new List<Product>
            {
                new Product {
                    Name = "Auriculares Logitech Audio H390 H390 negro",
                    Description = "Sonido envolvente, Cantidad de pares: 1. Con micrófono incorporado. El largo del cable es de 1.9m. Uso apto para profesional. Cómodos y prácticos. Tamaño del altavoz: 3.5mm",
                    Price = 36m,
                    Stock = 5,
                    TotalSold = 2,
                    CategoryId = auricularesId,
                    ImageUrl = "https://res.cloudinary.com/danl5ulmr/image/upload/v1766193299/auricular1_mfpyor.png" // Subí las imagenes directamente a Cloudinary por comodidad.
                },
                new Product {
                    Name = "Auriculares Inalámbricos Jbl Wave Buds 2 Negro",
                    Description = "Los auriculares JBL Wave Buds 2 incorporan el emocionante sonido JBL Pure Bass, además de cancelación de ruido activa y tecnología Smart Ambient para que decidas qué parte del mundo exterior no quieres que se te escape. Haz llamadas nítidas y claras sin manos con solo tocar los auriculares. Utiliza la aplicación JBL Headphones para personalizar el sonido y el idioma de los mensajes de voz. Conéctate sin problemas con hasta 8 dispositivos Bluetooth® y cambia sin esfuerzo de uno a otro. Y con hasta 40 horas* de reproducción, son un compañero de sonido para el día a día. (*con ANC desactivado) Contenido de la caja 1 par de auriculares JBL Wave Buds 2 1 cable de carga USB tipo C 3 tamaños de almohadilla 1 funda de carga 1 QSG/hoja de seguridad (S/i)",
                    Price = 56m,
                    Stock = 31,
                    TotalSold = 24,
                    CategoryId = auricularesId,
                    ImageUrl = "https://res.cloudinary.com/danl5ulmr/image/upload/v1766193298/auricular2_nlcktq.png"
                },
                new Product {
                    Name = "Jbl Tune 720bt negro",
                    Description = "Modo manos libres incluido. Incluye micrófono. La longitud del cable es de 1,2 m. Modelo T720BT. Color negro. Alcance 10m. Versión Bluetooth 5.3.",
                    Price = 68m,
                    Stock = 7,
                    TotalSold = 32,
                    CategoryId = auricularesId,
                    ImageUrl = "https://res.cloudinary.com/danl5ulmr/image/upload/v1766193299/auricular4_ehbsju.png"
                },
                new Product {
                    Name = "Auriculares Inalámbricos Aiwa Awknc1090 Cancelación De Ruido Color Negro",
                    Description = "Cantidad de pares: 1. Alcance inalámbrico de 10 m. La duración máxima de la batería es 12 h. Modo manos libres incluido. Con cancelación de ruido. Con micrófono incorporado. Resistentes al polvo. Uso apto para entretenimiento. La duración de la batería depende del uso que se le dé al producto. Tamaño del altavoz: 4cm.",
                    Price = 45m,
                    Stock = 3,
                    TotalSold = 6,
                    CategoryId = auricularesId,
                    ImageUrl = "https://res.cloudinary.com/danl5ulmr/image/upload/v1766193299/auricular3_eydn5v.png"
                },
                new Product {
                    Name = "Consola Sony PlayStation 5 Slim Blanco 4K 825GB Digital",
                    Description = "Capacidad: 825 GB. Incluye control. Resolución de 3840px x 2160px. Memoria RAM de 16GB. Cuenta con: almohadillas.",
                    Price = 599m,
                    Stock = 8,
                    TotalSold = 12,
                    CategoryId = consolasId,
                    ImageUrl = "https://res.cloudinary.com/danl5ulmr/image/upload/v1766193301/consola4_ublc76.png"
                },
                new Product {
                    Name = "Nintendo Switch OLED HEG-001 64GB Standard color neón 2021",
                    Description = "Capacidad: 64 GB. Incluye 2 controles. Resolución de 1920px x 1080px. Memoria RAM de 4GB. Tiene pantalla táctil. Cuenta con: 1 agarre para joy-con. La duración de la batería depende del uso que se le dé al producto.",
                    Price = 499m,
                    Stock = 32,
                    TotalSold = 51,
                    CategoryId = consolasId,
                    ImageUrl = "https://res.cloudinary.com/danl5ulmr/image/upload/v1766193300/consola1_pykpa7.png"
                },
                new Product {
                    Name = "Nintendo Switch 2 y Mario Kart World Bundle",
                    Description = "Capacidad: 256 GB. Pantalla táctil LCD de 7.9 con soporte para HDR y hasta 120 fps. Dock que admite 4K al conectarse a un televisor. Almacenamiento interno de 256 GB ampliable con microSD Express. Controladores Joy-Con 2 con conexión magnética. Incluye cable HDMI de ultra alta velocidad. Conectividad Bluetooth para emparejamiento inalámbrico.",
                    Price = 899m,
                    Stock = 5,
                    TotalSold = 2,
                    CategoryId = consolasId,
                    ImageUrl = "https://res.cloudinary.com/danl5ulmr/image/upload/v1766193300/consola2_xxzoff.png"
                },
                new Product {
                    Name = "Playstation 4 Slim 500gb - 2 Joystick - Fortnite Color Negro",
                    Description = "Capacidad de 500 GB para almacenar juegos y multimedia. Conectividad Wi-Fi y Bluetooth versión 4. Incluye 2 controles.",
                    Price = 350m,
                    Stock = 2,
                    TotalSold = 8,
                    CategoryId = consolasId,
                    ImageUrl = "https://res.cloudinary.com/danl5ulmr/image/upload/v1766193300/consola3_ak71ev.png"
                },
                new Product {
                    Name = "Nintendo Switch 2 Consola 2025 Videojuegos Gamer Negro",
                    Description = "Tamaño Aproximadamente 11,4 cm de alto x 27,9 cm de ancho x 1,4 cm de grosor (con los mandos Joy-Con™ 2 conectados). *El grosor máximo desde la punta de los joysticks hasta las partes salientes de los botones ZL/ZR es de 3 cm. Peso Aproximadamente 0,88 libras (aproximadamente 1,18 libras con los controladores Joy-Con 2 conectados) Pantalla Pantalla táctil capacitiva LCD de 7,9 pulgadas con amplia gama de colores, 1920 x 1080 píxeles, compatible con HDR10, VRR hasta 120 Hz CPU/GPU Procesador personalizado fabricado por NVIDIA. Almacenamiento 256 GB (UFS) *Una parte del almacenamiento está reservada para uso del sistema. Características de comunicación LAN inalámbrica (Wi-Fi 6) Bluetooth En el modo TV, Nintendo Switch 2 se puede conectar usando el puerto LAN con cable en la base. Salida de vídeo Salida a través del conector HDMI en modo TV Resolución máxima de 3840 x 2160 (4K) a 60 fps (modo TV) Admite 120 fps cuando se seleccionan resoluciones de 1920 x 1080/2560 x 1440 Admite HDR10 *Resolución máxima de 1920 x 1080 en modo de sobremesa y modo portátil, según la resolución de pantalla. Salida de audio Admite salida PCM lineal de 5.1 canales. Salida a través del conector HDMI en modo TV. *Se puede aplicar un efecto de sonido envolvente cuando se emite a auriculares o al altavoz incorporado (el efecto de sonido envolvente cuando se emite al altavoz incorporado requiere una actualización del sistema). Oradores Estéreo La estructura del gabinete independiente proporciona una calidad de sonido natural y clara. Micrófono Micrófono incorporado (monoaural) La cancelación de ruido, la cancelación de eco y el control automático de ganancia brindan una experiencia de chat de voz más cómoda. Botones Botón de encendido/botones de volumen Conectores USB-C® 2 conectores USB-C® El puerto inferior se usa para cargar la consola y conectarla a la base de Nintendo Switch 2. El puerto superior se usa para conectar accesorios o cargar la consola. Conector de audio Miniconector estéreo de 4 contactos y 3,5 mm (estándar CTIA) Nota: Nintendo no puede garantizar la funcionalidad con todos los productos. Ranura para tarjeta de juego Se pueden insertar tarjetas de juego tanto de Nintendo Switch 2 como de Nintendo Switch. ranura para tarjeta microSD Express Compatible solo con tarjetas microSD Express (hasta 2 TB) *Las tarjetas de memoria microSD que no son compatibles con microSD Express solo se pueden usar para copiar capturas de pantalla y videos de Nintendo Switch. Sensores Acelerómetro, giroscopio y sensor de mouse ubicados en los controladores Joy-Con 2 Sensor de brillo ubicado en la consola Entorno operativo 41-95 grados F / 20-80% de humedad Batería interna Batería de iones de litio/5220 mAh Duración de la batería Aprox. 2 – 6,5 horas *Estas son estimaciones aproximadas. La duración de la batería dependerá de los juegos que juegues. Tiempo de carga Aproximadamente 3 horas *Mientras el sistema esté en modo de suspensión. Base para Nintendo Switch 2 Tamaño Aproximadamente 4,5 pulgadas de alto x 7,9 pulgadas de ancho x 2 pulgadas de grosor. La altura incluye las 0,08 pulgadas agregadas por los pies en la parte inferior del muelle. Peso Aproximadamente 0,84 libras Puertos 2 puertos USB (compatibles con USB 2.0) en el lateral Conector del sistema Puerto adaptador de CA Puerto HDMI Puerto LAN Mandos Joy-Con™ 2 Tamaño Aproximadamente 4,57 pulgadas de alto x 0,56 pulgadas de ancho x 1,2 pulgadas de grosor *El grosor máximo desde la punta de los joysticks de control hasta las partes salientes de los botones ZL/ZR es de 1,2 pulgadas. Peso Joy-Con 2 [L] 2.3 oz Joy-Con 2 [R] 2.4 oz Botones Joy-Con 2 [L] Joystick izquierdo (presionable) Botones Arriba/Abajo/Izquierda/Derecha/L/ZL/SL/SR/- Botón de captura Botón de liberación Botón de sincronización Joy-Con 2 [R] Joystick derecho (presionable) Botones A/B/X/Y/R/ZR/SL/SR/+ Botón HOME Botón C Botón de liberación Botón de sincronización Inalámbrico Joy-Con 2 [L] Bluetooth Joy-Con 2 [R] Bluetooth/NFC Sensor Joy-Con 2 [L] Acelerómetro Giroscopio Sensor del mouse Joy-Con 2 [R] Acelerómetro Giroscopio Sensor del mouse Vibración Vibración HD 2 Batería interna Batería de iones de litio / capacidad de la batería 500 mAh Duración de la batería Aproximadamente 20 horas *La duración de la batería puede variar según el uso. Tiempo de carga Aproximadamente 3 horas y 30 minutos *Los controladores Joy-Con se cargan cuando se conectan al sistema o al soporte de carga Joy-Con 2.",
                    Price = 870m,
                    Stock = 0,
                    TotalSold = 11,
                    CategoryId = consolasId,
                    ImageUrl = "https://res.cloudinary.com/danl5ulmr/image/upload/v1766193300/consola5_ymodo3.png"
                },
                new Product {
                    Name = "Samsung Galaxy S25 5g 256 Gb",
                    Description = "Memoria RAM: 12 GB. Dispositivo liberado para que elijas la compañía telefónica que prefieras. Compatible con redes 5G. Pantalla Dynamic AMOLED 2X de 6.2. Cámara delantera de 10Mpx. Memoria interna de 256GB. Con reconocimiento facial y sensor de huella dactilar.",
                    Price = 880m,
                    Stock = 53,
                    TotalSold = 999,
                    CategoryId = celularesId,
                    ImageUrl = "https://res.cloudinary.com/danl5ulmr/image/upload/v1766193743/celular1_gnob19.png"
                },
                new Product {
                    Name = "Xiaomi Redmi Note 14 4g 6,67 256gb 8gb Ram Cámara 108mpx - Color Negro",
                    Description = "Dispositivo liberado para que elijas la compañía telefónica que prefieras. Pantalla AMOLED de 6.67. Tiene 3 cámaras traseras de 108 MP + 2 MP + 2 MP. Cámaras delanteras de f 2.4. Procesador Mediatek Helio G99 Ultra Octa-Core de 2.2GHz con 8GB de RAM. Batería de 5.5 Ah. Memoria interna de 256GB. Resistente al agua. Con reconocimiento facial y sensor de huella dactilar. Resistente al polvo.",
                    Price = 240m,
                    Stock = 5,
                    TotalSold = 61,
                    CategoryId = celularesId,
                    ImageUrl = "https://res.cloudinary.com/danl5ulmr/image/upload/v1766193299/celular2_jvjifc.png"
                },
                new Product {
                    Name = "Samsung Galaxy S25 Ultra 512gb Titanium Gray",
                    Description = "Memoria RAM: 12 GB. Dispositivo desbloqueado para que elijas la compañía telefónica que prefieras. Compatible con redes 5G. Pantalla de 6.9. Memoria interna de 512GB.",
                    Price = 1700m,
                    Stock = 0,
                    TotalSold = 42,
                    CategoryId = celularesId,
                    ImageUrl = "https://res.cloudinary.com/danl5ulmr/image/upload/v1766193299/celular3_zjadfg.png"
                },
                new Product {
                    Name = "Apple iPhone 16e (128 Gb) - Blanco",
                    Description = "Memoria RAM: 8 GB. Dispositivo desbloqueado para que elijas la compañía telefónica que prefieras. Pantalla de 6.1. Memoria interna de 128GB. Con reconocimiento facial.",
                    Price = 1024m,
                    Stock = 4,
                    TotalSold = 13,
                    CategoryId = celularesId,
                    ImageUrl = "https://res.cloudinary.com/danl5ulmr/image/upload/v1766193300/celular4_ce4gup.png"
                },
                new Product {
                    Name = "Samsung Galaxy A56 5G Dual Sim 256GB 12GB",
                    Description = "Dispositivo liberado para que elijas la compañía telefónica que prefieras. Compatible con redes 5G. Pantalla Super AMOLED de 6.7. Tiene 3 cámaras traseras de 50Mpx/12Mpx/5Mpx. Cámaras delanteras de 12Mpx. Batería de 5 Ah. Memoria interna de 256GB. A prueba de agua. Con reconocimiento facial y sensor de huella dactilar.",
                    Price = 510m,
                    Stock = 18,
                    TotalSold = 13,
                    CategoryId = celularesId,
                    ImageUrl = "https://res.cloudinary.com/danl5ulmr/image/upload/v1766193300/celular5_lsbvpe.png"
                },
                new Product {
                    Name = "Apple iPhone 13 (128 GB)",
                    Description = "Memoria RAM: 4 GB. Memoria interna: 128 GB. Pantalla Super Retina XDR de 6.1 pulgadas1. Modo Cine con baja profundidad de campo y cambios de enfoque automáticos en tus videos. Sistema avanzado de dos cámaras de 12 MP (gran angular y ultra gran angular) con Estilos Fotográficos, HDR Inteligente 4, modo Noche y grabación de video 4K HDR en Dolby Vision. Cámara frontal TrueDepth de 12 MP con modo Noche y grabación de video 4K HDR en Dolby Vision. Chip A15 Bionic para un rendimiento fuera de serie. Hasta 19 horas de reproducción de video2. Diseño resistente con Ceramic Shield. Resistencia al agua IP68, líder en la industria3. IOS 15 con nuevas funcionalidades para aprovechar tu iPhone al máximo4. Compatibilidad con accesorios MagSafe, que se acoplan fácilmente a tu iPhone y permiten una carga inalámbrica más rápida5.",
                    Price = 605m,
                    Stock = 12,
                    TotalSold = 5,
                    CategoryId = celularesId,
                    ImageUrl = "https://res.cloudinary.com/danl5ulmr/image/upload/v1766193300/celular5_lsbvpe.png"
                },
                new Product {
                    Name = "Apple iPhone 14 (128 GB)",
                    Description = "Pantalla Super Retina XDR de 6.1 pulgadas.(1) Sistema avanzado de cámaras para tomar mejores fotos en cualquier condición de luz. Modo Cine ahora en 4K Dolby Vision de hasta 30cps. Modo Acción para lograr videos estables, aún con cámara en mano. Detección de Choques(2), una funcionalidad de seguridad que pide ayuda cuando tú no puedes. Batería para todo el día y hasta 26 horas de reproducción de vídeo.(3) A15 Bionic, con GPU e 5 núcleos para un rendimiento fuera de serie. Red 5G ultrarrápida.(4) Ceramic Shield y resistencia al agua, características de durabilidad líderes en la industria.(5) IOS 16 ofrece aún más opciones de personalización y más formas de comunicarse y compartir.",
                    Price = 750m,
                    Stock = 5,
                    TotalSold = 4,
                    CategoryId = celularesId,
                    ImageUrl = "https://res.cloudinary.com/danl5ulmr/image/upload/v1766193299/celular7_xxsyzj.png"
                },
                new Product {
                    Name = "Tarjeta Video Gigabyte Amd Radeon Rx 9060 Xt Gaming Oc 16gb",
                    Description = "Tamaño de la memoria: 16 GB. Potencia gráfica total de 160 W para un rendimiento óptimo en juegos. Interfaz PCIe 5.0 para alta velocidad de transferencia. Bus de memoria de 128 bits que mejora el ancho de banda. Resolución máxima de 8K para imágenes ultra nítidas. Compatible con DirectX y OpenGL para un amplio soporte de juegos. Refrigeración por aire que garantiza un funcionamiento eficiente.",
                    Price = 760m,
                    Stock = 21,
                    TotalSold = 82,
                    CategoryId = graficasId,
                    ImageUrl = "https://res.cloudinary.com/danl5ulmr/image/upload/v1766193301/grafica1_bjcm9m.png"
                },
                new Product {
                    Name = "Tarjeta Gráfica MSI GeForce RTX 3060 Ventus 12GB GDDR6 PCI Express 4.0",
                    Description = "Tamaño de la memoria: 12 GB. Interfaz PCI-Express 4.0. Memoria gráfica GDDR6 de 15000MHz. Bus de memoria: 192bit. Cantidad de núcleos: 3584. Frecuencia boost del núcleo de 1807MHz y base de 1320MHz. Resolución máxima: 8K. Compatible con directX y openGL. Requiere de 550W de alimentación. Cuenta con conexión: 3 DisplayPort 1.4. Incluye: Manual de usuario. Ideal para trabajar a alta velocidad. Admite hasta 4 monitores.",
                    Price = 470m,
                    Stock = 5,
                    TotalSold = 56,
                    CategoryId = graficasId,
                    ImageUrl = "https://res.cloudinary.com/danl5ulmr/image/upload/v1766193301/grafica2_oduauv.png"
                },
                new Product {
                    Name = "Placa De Video Palit Geforce Rtx 5060 Ti Dual 8gb Gddr7 Vx",
                    Description = "Tamaño de la memoria: 8 GB. Interfaz PCI-Express 5.0 para máxima compatibilidad y velocidad. Resolución máxima de 8K para imágenes detalladas. Bus de memoria de 128 bits para mayor ancho de banda. Conectividad con 4 salidas para múltiples monitores. Refrigeración por aire para un rendimiento óptimo. Compatible con DirectX y OpenGL para aplicaciones gráficas avanzadas.",
                    Price = 670m,
                    Stock = 8,
                    TotalSold = 44,
                    CategoryId = graficasId,
                    ImageUrl = "https://res.cloudinary.com/danl5ulmr/image/upload/v1766193301/grafica3_fda8ul.png"
                },
                new Product {
                    Name = "Placa De Video Geforce Rtx 5070 12gb Msi Gaming Trio Oc 1",
                    Description = "Tamaño de la memoria: 12 GB. Interfaz PCI Express® Gen 5 para máxima velocidad de transferencia de datos. Bus de memoria de 192 bit para mayor ancho de banda. Compatible con DirectX para aprovechar gráficos avanzados. Cuatro salidas de monitor para configuraciones de múltiples pantallas. Requerimiento energético de 650 W para asegurar un rendimiento óptimo.",
                    Price = 1200m,
                    Stock = 2,
                    TotalSold = 15,
                    CategoryId = graficasId,
                    ImageUrl = "https://res.cloudinary.com/danl5ulmr/image/upload/v1766193301/grafica4_e9qumc.png"
                },
                new Product {
                    Name = "Tarjeta De Video Gigabyte Amd Radeon Rx 9060 Xt Gaming Oc 8g",
                    Description = "Tamaño de la memoria: 8 GB. Interfaz PCI-E 5.0 para una conexión de alta velocidad y eficiencia. Potencia gráfica total de 250 W para un rendimiento óptimo. Soporta resolución máxima de 8K para imágenes ultra nítidas. Incluye tres salidas de monitor para múltiples display. Conectividad HDMI y DisplayPort para versatilidad en el uso. Refrigeración por aire para un funcionamiento estable y eficiente.",
                    Price = 660m,
                    Stock = 7,
                    TotalSold = 51,
                    CategoryId = graficasId,
                    ImageUrl = "https://res.cloudinary.com/danl5ulmr/image/upload/v1766193301/grafica6_dxzivj.png"
                },
                new Product {
                    Name = "HyperX Cloud Stinger 2 Core Negro",
                    Description = "Auriculares gamer livianos con micrófono, buena aislación y sonido estéreo claro para juegos y llamadas.",
                    Price = 49m,
                    Stock = 15,
                    TotalSold = 18,
                    CategoryId = auricularesId,
                    ImageUrl = "https://res.cloudinary.com/danl5ulmr/image/upload/v1766193299/auricular3_eydn5v.png"
                },
                new Product {
                    Name = "Sony WH-CH520 Bluetooth Negro",
                    Description = "Auriculares inalámbricos con batería de larga duración, sonido equilibrado y conexión estable.",
                    Price = 69m,
                    Stock = 20,
                    TotalSold = 40,
                    CategoryId = auricularesId,
                    ImageUrl = "https://res.cloudinary.com/danl5ulmr/image/upload/v1766193298/auricular2_nlcktq.png"
                },
                new Product {
                    Name = "Apple AirPods 2da Gen",
                    Description = "Auriculares true wireless con estuche de carga, emparejamiento rápido y audio nítido para llamadas.",
                    Price = 129m,
                    Stock = 10,
                    TotalSold = 55,
                    CategoryId = auricularesId,
                    ImageUrl = "https://res.cloudinary.com/danl5ulmr/image/upload/v1766193299/auricular4_ehbsju.png"
                },
                new Product {
                    Name = "Razer BlackShark V2 X",
                    Description = "Auriculares gamer con micrófono cardioide, drivers grandes y buena comodidad para sesiones largas.",
                    Price = 79m,
                    Stock = 12,
                    TotalSold = 28,
                    CategoryId = auricularesId,
                    ImageUrl = "https://res.cloudinary.com/danl5ulmr/image/upload/v1766193299/auricular1_mfpyor.png"
                },
                new Product {
                    Name = "JBL Quantum 100",
                    Description = "Auriculares gamer con sonido envolvente y micrófono desmontable. Buen balance precio/beneficio.",
                    Price = 59m,
                    Stock = 18,
                    TotalSold = 22,
                    CategoryId = auricularesId,
                    ImageUrl = "https://res.cloudinary.com/danl5ulmr/image/upload/v1766193298/auricular2_nlcktq.png"
                },
                new Product {
                    Name = "Logitech G435 Lightspeed",
                    Description = "Auriculares inalámbricos ligeros, compatibilidad con PC/PS y conexión Lightspeed/Bluetooth.",
                    Price = 89m,
                    Stock = 9,
                    TotalSold = 16,
                    CategoryId = auricularesId,
                    ImageUrl = "https://res.cloudinary.com/danl5ulmr/image/upload/v1766193299/auricular1_mfpyor.png"
                },
                new Product {
                    Name = "Xbox Series X 1TB",
                    Description = "Consola 4K con 1TB, alto rendimiento y tiempos de carga rápidos con SSD.",
                    Price = 649m,
                    Stock = 6,
                    TotalSold = 35,
                    CategoryId = consolasId,
                    ImageUrl = "https://res.cloudinary.com/danl5ulmr/image/upload/v1766193301/consola4_ublc76.png"
                },
                new Product {
                    Name = "Xbox Series S 512GB",
                    Description = "Consola digital compacta, ideal para Game Pass. Excelente rendimiento a precio accesible.",
                    Price = 349m,
                    Stock = 14,
                    TotalSold = 47,
                    CategoryId = consolasId,
                    ImageUrl = "https://res.cloudinary.com/danl5ulmr/image/upload/v1766193301/consola4_ublc76.png"
                },
                new Product {
                    Name = "PlayStation 5 Standard 1TB",
                    Description = "Edición con lector de discos, 4K, SSD rápido y gran catálogo de exclusivos.",
                    Price = 699m,
                    Stock = 4,
                    TotalSold = 26,
                    CategoryId = consolasId,
                    ImageUrl = "https://res.cloudinary.com/danl5ulmr/image/upload/v1766193300/consola1_pykpa7.png"
                },
                new Product {
                    Name = "Nintendo Switch Lite Turquesa",
                    Description = "Consola portátil compacta para juegos de Switch. Ideal para viajar y jugar en movimiento.",
                    Price = 249m,
                    Stock = 22,
                    TotalSold = 33,
                    CategoryId = consolasId,
                    ImageUrl = "https://res.cloudinary.com/danl5ulmr/image/upload/v1766193300/consola2_xxzoff.png"
                },
                new Product {
                    Name = "Steam Deck 512GB",
                    Description = "PC portátil para juegos con SteamOS. Gran rendimiento para biblioteca de Steam.",
                    Price = 799m,
                    Stock = 5,
                    TotalSold = 9,
                    CategoryId = consolasId,
                    ImageUrl = "https://res.cloudinary.com/danl5ulmr/image/upload/v1766193300/consola5_ymodo3.png"
                },
                new Product {
                    Name = "PlayStation 4 Pro 1TB",
                    Description = "Consola 4K (checkerboard), 1TB y muy buen rendimiento para catálogo clásico de PS4.",
                    Price = 420m,
                    Stock = 3,
                    TotalSold = 14,
                    CategoryId = consolasId,
                    ImageUrl = "https://res.cloudinary.com/danl5ulmr/image/upload/v1766193300/consola3_ak71ev.png"
                },
                new Product {
                    Name = "HyperX Cloud II Rojo",
                    Description = "Auriculares gamer con sonido envolvente virtual 7.1 y micrófono desmontable.",
                    Price = 89m,
                    Stock = 14,
                    TotalSold = 46,
                    CategoryId = auricularesId,
                    ImageUrl = "https://res.cloudinary.com/danl5ulmr/image/upload/v1766193299/auricular4_ehbsju.png"
                },
                new Product {
                    Name = "Sony WH-1000XM4",
                    Description = "Auriculares premium con cancelación activa de ruido y excelente autonomía.",
                    Price = 299m,
                    Stock = 6,
                    TotalSold = 88,
                    CategoryId = auricularesId,
                    ImageUrl = "https://res.cloudinary.com/danl5ulmr/image/upload/v1766193298/auricular2_nlcktq.png"
                },
                new Product {
                    Name = "Logitech G733 Lightspeed",
                    Description = "Auriculares inalámbricos RGB con sonido balanceado y micrófono Blue VO!CE.",
                    Price = 129m,
                    Stock = 11,
                    TotalSold = 32,
                    CategoryId = auricularesId,
                    ImageUrl = "https://res.cloudinary.com/danl5ulmr/image/upload/v1766193299/auricular1_mfpyor.png"
                },
                new Product {
                    Name = "JBL Tune 510BT",
                    Description = "Auriculares bluetooth livianos con sonido JBL Pure Bass.",
                    Price = 45m,
                    Stock = 22,
                    TotalSold = 54,
                    CategoryId = auricularesId,
                    ImageUrl = "https://res.cloudinary.com/danl5ulmr/image/upload/v1766193299/auricular3_eydn5v.png"
                },
                new Product {
                    Name = "Corsair HS55 Stereo",
                    Description = "Auriculares cómodos con micrófono omnidireccional y buena claridad de audio.",
                    Price = 59m,
                    Stock = 9,
                    TotalSold = 21,
                    CategoryId = auricularesId,
                    ImageUrl = "https://res.cloudinary.com/danl5ulmr/image/upload/v1766193299/auricular1_mfpyor.png"
                },
                new Product {
                    Name = "SteelSeries Arctis 1",
                    Description = "Auriculares versátiles para PC, consola y mobile.",
                    Price = 79m,
                    Stock = 7,
                    TotalSold = 27,
                    CategoryId = auricularesId,
                    ImageUrl = "https://res.cloudinary.com/danl5ulmr/image/upload/v1766193298/auricular2_nlcktq.png"
                },
                new Product {
                    Name = "Google Pixel 9 256GB",
                    Description = "Android premium con gran cámara y software optimizado. 256GB de almacenamiento.",
                    Price = 899m,
                    Stock = 7,
                    TotalSold = 19,
                    CategoryId = celularesId,
                    ImageUrl = "https://res.cloudinary.com/danl5ulmr/image/upload/v1766193299/celular2_jvjifc.png"
                },
                new Product {
                    Name = "Samsung Galaxy S24 256GB",
                    Description = "Pantalla AMOLED, gran rendimiento y cámaras versátiles. 256GB de almacenamiento.",
                    Price = 790m,
                    Stock = 11,
                    TotalSold = 41,
                    CategoryId = celularesId,
                    ImageUrl = "https://res.cloudinary.com/danl5ulmr/image/upload/v1766193300/celular4_ce4gup.png"
                },
                new Product {
                    Name = "iPhone 15 128GB Negro",
                    Description = "Rendimiento fluido, buena cámara, ecosistema Apple y excelente calidad de pantalla.",
                    Price = 920m,
                    Stock = 6,
                    TotalSold = 27,
                    CategoryId = celularesId,
                    ImageUrl = "https://res.cloudinary.com/danl5ulmr/image/upload/v1766193300/celular5_lsbvpe.png"
                },
                new Product {
                    Name = "Xiaomi Poco X6 Pro 256GB",
                    Description = "Excelente relación precio/rendimiento, pantalla fluida y buena batería.",
                    Price = 399m,
                    Stock = 19,
                    TotalSold = 38,
                    CategoryId = celularesId,
                    ImageUrl = "https://res.cloudinary.com/danl5ulmr/image/upload/v1766193299/celular7_xxsyzj.png"
                },
                new Product {
                    Name = "Motorola Edge 50 256GB",
                    Description = "Diseño premium, buena pantalla y cámaras completas. 256GB de almacenamiento.",
                    Price = 540m,
                    Stock = 10,
                    TotalSold = 12,
                    CategoryId = celularesId,
                    ImageUrl = "https://res.cloudinary.com/danl5ulmr/image/upload/v1766193299/celular7_xxsyzj.png"
                },
                new Product {
                    Name = "Samsung Galaxy A35 5G 128GB",
                    Description = "Gama media equilibrada, pantalla AMOLED y batería duradera. Compatible 5G.",
                    Price = 320m,
                    Stock = 25,
                    TotalSold = 29,
                    CategoryId = celularesId,
                    ImageUrl = "https://res.cloudinary.com/danl5ulmr/image/upload/v1766193743/celular1_gnob19.png"
                },
                new Product {
                    Name = "Xbox Series S Carbon Black 1TB",
                    Description = "Versión mejorada con más almacenamiento para juegos digitales.",
                    Price = 399m,
                    Stock = 10,
                    TotalSold = 39,
                    CategoryId = consolasId,
                    ImageUrl = "https://res.cloudinary.com/danl5ulmr/image/upload/v1766193300/consola1_pykpa7.png"
                },
                new Product {
                    Name = "PlayStation 5 Digital Edition",
                    Description = "PS5 sin lector de discos, ideal para biblioteca digital.",
                    Price = 579m,
                    Stock = 5,
                    TotalSold = 44,
                    CategoryId = consolasId,
                    ImageUrl = "https://res.cloudinary.com/danl5ulmr/image/upload/v1766193301/consola4_ublc76.png"
                },
                new Product {
                    Name = "Nintendo Switch Lite Gris",
                    Description = "Consola portátil compacta enfocada en juego individual.",
                    Price = 229m,
                    Stock = 19,
                    TotalSold = 61,
                    CategoryId = consolasId,
                    ImageUrl = "https://res.cloudinary.com/danl5ulmr/image/upload/v1766193300/consola3_ak71ev.png"
                },
                new Product {
                    Name = "Steam Deck OLED 1TB",
                    Description = "PC portátil para juegos con pantalla OLED y gran rendimiento.",
                    Price = 899m,
                    Stock = 3,
                    TotalSold = 18,
                    CategoryId = consolasId,
                    ImageUrl = "https://res.cloudinary.com/danl5ulmr/image/upload/v1766193300/consola2_xxzoff.png"
                },
                new Product {
                    Name = "PlayStation 4 Slim 1TB",
                    Description = "Clásica consola con gran catálogo de juegos.",
                    Price = 330m,
                    Stock = 6,
                    TotalSold = 52,
                    CategoryId = consolasId,
                    ImageUrl = "https://res.cloudinary.com/danl5ulmr/image/upload/v1766193300/consola3_ak71ev.png"
                },
                new Product {
                    Name = "Xbox One X 1TB",
                    Description = "Consola potente de generación anterior con soporte 4K.",
                    Price = 310m,
                    Stock = 4,
                    TotalSold = 17,
                    CategoryId = consolasId,
                    ImageUrl = "https://res.cloudinary.com/danl5ulmr/image/upload/v1766193300/consola5_ymodo3.png"
                },
                new Product {
                    Name = "iPhone 15 Pro 256GB",
                    Description = "Diseño premium, alto rendimiento y cámaras avanzadas.",
                    Price = 1299m,
                    Stock = 4,
                    TotalSold = 64,
                    CategoryId = celularesId,
                    ImageUrl = "https://res.cloudinary.com/danl5ulmr/image/upload/v1766193743/celular1_gnob19.png"
                },
                new Product {
                    Name = "Samsung Galaxy Z Flip 6",
                    Description = "Smartphone plegable compacto con pantalla AMOLED.",
                    Price = 999m,
                    Stock = 3,
                    TotalSold = 22,
                    CategoryId = celularesId,
                    ImageUrl = "https://res.cloudinary.com/danl5ulmr/image/upload/v1766193299/celular3_zjadfg.png"
                },
                new Product {
                    Name = "Xiaomi Redmi 13 Pro",
                    Description = "Gama media con gran pantalla y buena batería.",
                    Price = 299m,
                    Stock = 21,
                    TotalSold = 48,
                    CategoryId = celularesId,
                    ImageUrl = "https://res.cloudinary.com/danl5ulmr/image/upload/v1766193299/celular2_jvjifc.png"
                },
                new Product {
                    Name = "Motorola G84 5G",
                    Description = "Diseño moderno, pantalla OLED y conectividad 5G.",
                    Price = 269m,
                    Stock = 17,
                    TotalSold = 33,
                    CategoryId = celularesId,
                    ImageUrl = "https://res.cloudinary.com/danl5ulmr/image/upload/v1766193300/celular5_lsbvpe.png"
                },
                new Product {
                    Name = "Samsung Galaxy A15",
                    Description = "Equipo accesible con buena autonomía y pantalla grande.",
                    Price = 199m,
                    Stock = 30,
                    TotalSold = 58,
                    CategoryId = celularesId,
                    ImageUrl = "https://res.cloudinary.com/danl5ulmr/image/upload/v1766193300/celular4_ce4gup.png"
                },
                new Product {
                    Name = "Apple iPhone 12 128GB",
                    Description = "iPhone compacto con excelente rendimiento y cámara.",
                    Price = 599m,
                    Stock = 8,
                    TotalSold = 41,
                    CategoryId = celularesId,
                    ImageUrl = "https://res.cloudinary.com/danl5ulmr/image/upload/v1766193299/celular7_xxsyzj.png"
                },
                new Product {
                    Name = "RTX 4090 24GB",
                    Description = "GPU tope de gama para gaming 4K y tareas profesionales.",
                    Price = 1999m,
                    Stock = 2,
                    TotalSold = 12,
                    CategoryId = graficasId,
                    ImageUrl = "https://res.cloudinary.com/danl5ulmr/image/upload/v1766193301/grafica4_e9qumc.png"
                },
                new Product {
                    Name = "RTX 4080 Super 16GB",
                    Description = "Alto rendimiento para gaming exigente y creación de contenido.",
                    Price = 1399m,
                    Stock = 4,
                    TotalSold = 19,
                    CategoryId = graficasId,
                    ImageUrl = "https://res.cloudinary.com/danl5ulmr/image/upload/v1766193301/grafica3_fda8ul.png"
                },
                new Product {
                    Name = "RX 7900 XTX 24GB",
                    Description = "Potente GPU AMD con mucha VRAM y gran rendimiento.",
                    Price = 1099m,
                    Stock = 6,
                    TotalSold = 23,
                    CategoryId = graficasId,
                    ImageUrl = "https://res.cloudinary.com/danl5ulmr/image/upload/v1766193301/grafica1_bjcm9m.png"
                },
                new Product {
                    Name = "RX 6700 XT 12GB",
                    Description = "Excelente opción para 1440p con buena eficiencia.",
                    Price = 429m,
                    Stock = 11,
                    TotalSold = 37,
                    CategoryId = graficasId,
                    ImageUrl = "https://res.cloudinary.com/danl5ulmr/image/upload/v1766193301/grafica2_oduauv.png"
                },
                new Product {
                    Name = "RTX 3050 8GB",
                    Description = "Entrada al gaming RTX con DLSS y ray tracing.",
                    Price = 289m,
                    Stock = 18,
                    TotalSold = 49,
                    CategoryId = graficasId,
                    ImageUrl = "https://res.cloudinary.com/danl5ulmr/image/upload/v1766193301/grafica6_dxzivj.png"
                },
                new Product {
                    Name = "GTX 1660 Super 6GB",
                    Description = "GPU clásica para 1080p, aún muy utilizada.",
                    Price = 249m,
                    Stock = 13,
                    TotalSold = 34,
                    CategoryId = graficasId,
                    ImageUrl = "https://res.cloudinary.com/danl5ulmr/image/upload/v1766193301/grafica2_oduauv.png"
                },
                new Product {
                    Name = "NVIDIA GeForce RTX 4070 Super 12GB",
                    Description = "Gran rendimiento 1440p, DLSS y eficiencia energética. Ideal para gaming y creación.",
                    Price = 890m,
                    Stock = 6,
                    TotalSold = 21,
                    CategoryId = graficasId,
                    ImageUrl = "https://res.cloudinary.com/danl5ulmr/image/upload/v1766193301/grafica1_bjcm9m.png"
                },
                new Product {
                    Name = "NVIDIA GeForce RTX 4060 8GB",
                    Description = "Buena opción para 1080p/1440p con DLSS. Consumo moderado y buen desempeño.",
                    Price = 420m,
                    Stock = 14,
                    TotalSold = 44,
                    CategoryId = graficasId,
                    ImageUrl = "https://res.cloudinary.com/danl5ulmr/image/upload/v1766193301/grafica1_bjcm9m.png"
                },
                new Product {
                    Name = "AMD Radeon RX 7800 XT 16GB",
                    Description = "Excelente 1440p, 16GB VRAM y gran relación precio/rendimiento.",
                    Price = 650m,
                    Stock = 9,
                    TotalSold = 25,
                    CategoryId = graficasId,
                    ImageUrl = "https://res.cloudinary.com/danl5ulmr/image/upload/v1766193301/grafica4_e9qumc.png"
                },
                new Product {
                    Name = "AMD Radeon RX 7600 8GB",
                    Description = "GPU sólida para 1080p con buen rendimiento y eficiencia.",
                    Price = 310m,
                    Stock = 17,
                    TotalSold = 31,
                    CategoryId = graficasId,
                    ImageUrl = "https://res.cloudinary.com/danl5ulmr/image/upload/v1766193301/grafica3_fda8ul.png"
                },
                new Product {
                    Name = "NVIDIA GeForce RTX 5080 16GB",
                    Description = "Gama alta para 4K, alto rendimiento y soporte avanzado de tecnologías modernas.",
                    Price = 1490m,
                    Stock = 3,
                    TotalSold = 8,
                    CategoryId = graficasId,
                    ImageUrl = "https://res.cloudinary.com/danl5ulmr/image/upload/v1766193301/grafica3_fda8ul.png"
                },
                new Product {
                    Name = "NVIDIA GeForce RTX 3060 Ti 8GB",
                    Description = "Muy buena para 1080p/1440p, opción popular con gran desempeño general.",
                    Price = 390m,
                    Stock = 8,
                    TotalSold = 36,
                    CategoryId = graficasId,
                    ImageUrl = "https://res.cloudinary.com/danl5ulmr/image/upload/v1766193301/grafica1_bjcm9m.png"
                },
            };

            context.Products.AddRange(products);
            await context.SaveChangesAsync();
        }
    }
}
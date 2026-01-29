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
                    CategoryId = auricularesId,
                    ImageUrl = "https://res.cloudinary.com/danl5ulmr/image/upload/v1766193299/auricular1_mfpyor.png" // Subí las imagenes directamente a Cloudinary por comodidad.
                },
                new Product {
                    Name = "Auriculares Inalámbricos Jbl Wave Buds 2 Negro",
                    Description = "Los auriculares JBL Wave Buds 2 incorporan el emocionante sonido JBL Pure Bass, además de cancelación de ruido activa y tecnología Smart Ambient para que decidas qué parte del mundo exterior no quieres que se te escape. Haz llamadas nítidas y claras sin manos con solo tocar los auriculares. Utiliza la aplicación JBL Headphones para personalizar el sonido y el idioma de los mensajes de voz. Conéctate sin problemas con hasta 8 dispositivos Bluetooth® y cambia sin esfuerzo de uno a otro. Y con hasta 40 horas* de reproducción, son un compañero de sonido para el día a día. (*con ANC desactivado) Contenido de la caja 1 par de auriculares JBL Wave Buds 2 1 cable de carga USB tipo C 3 tamaños de almohadilla 1 funda de carga 1 QSG/hoja de seguridad (S/i)",
                    Price = 56m,
                    Stock = 31,
                    CategoryId = auricularesId,
                    ImageUrl = "https://res.cloudinary.com/danl5ulmr/image/upload/v1766193298/auricular2_nlcktq.png"
                },
                new Product {
                    Name = "Jbl Tune 720bt negro",
                    Description = "Modo manos libres incluido. Incluye micrófono. La longitud del cable es de 1,2 m. Modelo T720BT. Color negro. Alcance 10m. Versión Bluetooth 5.3.",
                    Price = 68m,
                    Stock = 7,
                    CategoryId = auricularesId,
                    ImageUrl = "https://res.cloudinary.com/danl5ulmr/image/upload/v1766193299/auricular4_ehbsju.png"
                },
                new Product {
                    Name = "Auriculares Inalámbricos Aiwa Awknc1090 Cancelación De Ruido Color Negro",
                    Description = "Cantidad de pares: 1. Alcance inalámbrico de 10 m. La duración máxima de la batería es 12 h. Modo manos libres incluido. Con cancelación de ruido. Con micrófono incorporado. Resistentes al polvo. Uso apto para entretenimiento. La duración de la batería depende del uso que se le dé al producto. Tamaño del altavoz: 4cm.",
                    Price = 45m,
                    Stock = 3,
                    CategoryId = auricularesId,
                    ImageUrl = "https://res.cloudinary.com/danl5ulmr/image/upload/v1766193299/auricular3_eydn5v.png"
                },
                new Product {
                    Name = "Consola Sony PlayStation 5 Slim Blanco 4K 825GB Digital",
                    Description = "Capacidad: 825 GB. Incluye control. Resolución de 3840px x 2160px. Memoria RAM de 16GB. Cuenta con: almohadillas.",
                    Price = 599m,
                    Stock = 8,
                    CategoryId = consolasId,
                    ImageUrl = "https://res.cloudinary.com/danl5ulmr/image/upload/v1766193301/consola4_ublc76.png"
                },
                new Product {
                    Name = "Nintendo Switch OLED HEG-001 64GB Standard color neón 2021",
                    Description = "Capacidad: 64 GB. Incluye 2 controles. Resolución de 1920px x 1080px. Memoria RAM de 4GB. Tiene pantalla táctil. Cuenta con: 1 agarre para joy-con. La duración de la batería depende del uso que se le dé al producto.",
                    Price = 499m,
                    Stock = 32,
                    CategoryId = consolasId,
                    ImageUrl = "https://res.cloudinary.com/danl5ulmr/image/upload/v1766193300/consola1_pykpa7.png"
                },
                new Product {
                    Name = "Nintendo Switch 2 y Mario Kart World Bundle",
                    Description = "Capacidad: 256 GB. Pantalla táctil LCD de 7.9 con soporte para HDR y hasta 120 fps. Dock que admite 4K al conectarse a un televisor. Almacenamiento interno de 256 GB ampliable con microSD Express. Controladores Joy-Con 2 con conexión magnética. Incluye cable HDMI de ultra alta velocidad. Conectividad Bluetooth para emparejamiento inalámbrico.",
                    Price = 899m,
                    Stock = 5,
                    CategoryId = consolasId,
                    ImageUrl = "https://res.cloudinary.com/danl5ulmr/image/upload/v1766193300/consola2_xxzoff.png"
                },
                new Product {
                    Name = "Playstation 4 Slim 500gb - 2 Joystick - Fortnite Color Negro",
                    Description = "Capacidad de 500 GB para almacenar juegos y multimedia. Conectividad Wi-Fi y Bluetooth versión 4. Incluye 2 controles.",
                    Price = 350m,
                    Stock = 2,
                    CategoryId = consolasId,
                    ImageUrl = "https://res.cloudinary.com/danl5ulmr/image/upload/v1766193300/consola3_ak71ev.png"
                },
                new Product {
                    Name = "Nintendo Switch 2 Consola 2025 Videojuegos Gamer Negro",
                    Description = "Tamaño Aproximadamente 11,4 cm de alto x 27,9 cm de ancho x 1,4 cm de grosor (con los mandos Joy-Con™ 2 conectados). *El grosor máximo desde la punta de los joysticks hasta las partes salientes de los botones ZL/ZR es de 3 cm. Peso Aproximadamente 0,88 libras (aproximadamente 1,18 libras con los controladores Joy-Con 2 conectados) Pantalla Pantalla táctil capacitiva LCD de 7,9 pulgadas con amplia gama de colores, 1920 x 1080 píxeles, compatible con HDR10, VRR hasta 120 Hz CPU/GPU Procesador personalizado fabricado por NVIDIA. Almacenamiento 256 GB (UFS) *Una parte del almacenamiento está reservada para uso del sistema. Características de comunicación LAN inalámbrica (Wi-Fi 6) Bluetooth En el modo TV, Nintendo Switch 2 se puede conectar usando el puerto LAN con cable en la base. Salida de vídeo Salida a través del conector HDMI en modo TV Resolución máxima de 3840 x 2160 (4K) a 60 fps (modo TV) Admite 120 fps cuando se seleccionan resoluciones de 1920 x 1080/2560 x 1440 Admite HDR10 *Resolución máxima de 1920 x 1080 en modo de sobremesa y modo portátil, según la resolución de pantalla. Salida de audio Admite salida PCM lineal de 5.1 canales. Salida a través del conector HDMI en modo TV. *Se puede aplicar un efecto de sonido envolvente cuando se emite a auriculares o al altavoz incorporado (el efecto de sonido envolvente cuando se emite al altavoz incorporado requiere una actualización del sistema). Oradores Estéreo La estructura del gabinete independiente proporciona una calidad de sonido natural y clara. Micrófono Micrófono incorporado (monoaural) La cancelación de ruido, la cancelación de eco y el control automático de ganancia brindan una experiencia de chat de voz más cómoda. Botones Botón de encendido/botones de volumen Conectores USB-C® 2 conectores USB-C® El puerto inferior se usa para cargar la consola y conectarla a la base de Nintendo Switch 2. El puerto superior se usa para conectar accesorios o cargar la consola. Conector de audio Miniconector estéreo de 4 contactos y 3,5 mm (estándar CTIA) Nota: Nintendo no puede garantizar la funcionalidad con todos los productos. Ranura para tarjeta de juego Se pueden insertar tarjetas de juego tanto de Nintendo Switch 2 como de Nintendo Switch. ranura para tarjeta microSD Express Compatible solo con tarjetas microSD Express (hasta 2 TB) *Las tarjetas de memoria microSD que no son compatibles con microSD Express solo se pueden usar para copiar capturas de pantalla y videos de Nintendo Switch. Sensores Acelerómetro, giroscopio y sensor de mouse ubicados en los controladores Joy-Con 2 Sensor de brillo ubicado en la consola Entorno operativo 41-95 grados F / 20-80% de humedad Batería interna Batería de iones de litio/5220 mAh Duración de la batería Aprox. 2 – 6,5 horas *Estas son estimaciones aproximadas. La duración de la batería dependerá de los juegos que juegues. Tiempo de carga Aproximadamente 3 horas *Mientras el sistema esté en modo de suspensión. Base para Nintendo Switch 2 Tamaño Aproximadamente 4,5 pulgadas de alto x 7,9 pulgadas de ancho x 2 pulgadas de grosor. La altura incluye las 0,08 pulgadas agregadas por los pies en la parte inferior del muelle. Peso Aproximadamente 0,84 libras Puertos 2 puertos USB (compatibles con USB 2.0) en el lateral Conector del sistema Puerto adaptador de CA Puerto HDMI Puerto LAN Mandos Joy-Con™ 2 Tamaño Aproximadamente 4,57 pulgadas de alto x 0,56 pulgadas de ancho x 1,2 pulgadas de grosor *El grosor máximo desde la punta de los joysticks de control hasta las partes salientes de los botones ZL/ZR es de 1,2 pulgadas. Peso Joy-Con 2 [L] 2.3 oz Joy-Con 2 [R] 2.4 oz Botones Joy-Con 2 [L] Joystick izquierdo (presionable) Botones Arriba/Abajo/Izquierda/Derecha/L/ZL/SL/SR/- Botón de captura Botón de liberación Botón de sincronización Joy-Con 2 [R] Joystick derecho (presionable) Botones A/B/X/Y/R/ZR/SL/SR/+ Botón HOME Botón C Botón de liberación Botón de sincronización Inalámbrico Joy-Con 2 [L] Bluetooth Joy-Con 2 [R] Bluetooth/NFC Sensor Joy-Con 2 [L] Acelerómetro Giroscopio Sensor del mouse Joy-Con 2 [R] Acelerómetro Giroscopio Sensor del mouse Vibración Vibración HD 2 Batería interna Batería de iones de litio / capacidad de la batería 500 mAh Duración de la batería Aproximadamente 20 horas *La duración de la batería puede variar según el uso. Tiempo de carga Aproximadamente 3 horas y 30 minutos *Los controladores Joy-Con se cargan cuando se conectan al sistema o al soporte de carga Joy-Con 2.",
                    Price = 870m,
                    Stock = 0,
                    CategoryId = consolasId,
                    ImageUrl = "https://res.cloudinary.com/danl5ulmr/image/upload/v1766193300/consola5_ymodo3.png"
                },
                new Product {
                    Name = "Samsung Galaxy S25 5g 256 Gb",
                    Description = "Memoria RAM: 12 GB. Dispositivo liberado para que elijas la compañía telefónica que prefieras. Compatible con redes 5G. Pantalla Dynamic AMOLED 2X de 6.2. Cámara delantera de 10Mpx. Memoria interna de 256GB. Con reconocimiento facial y sensor de huella dactilar.",
                    Price = 880m,
                    Stock = 53,
                    CategoryId = celularesId,
                    ImageUrl = "https://res.cloudinary.com/danl5ulmr/image/upload/v1766193743/celular1_gnob19.png"
                },
                new Product {
                    Name = "Xiaomi Redmi Note 14 4g 6,67 256gb 8gb Ram Cámara 108mpx - Color Negro",
                    Description = "Dispositivo liberado para que elijas la compañía telefónica que prefieras. Pantalla AMOLED de 6.67. Tiene 3 cámaras traseras de 108 MP + 2 MP + 2 MP. Cámaras delanteras de f 2.4. Procesador Mediatek Helio G99 Ultra Octa-Core de 2.2GHz con 8GB de RAM. Batería de 5.5 Ah. Memoria interna de 256GB. Resistente al agua. Con reconocimiento facial y sensor de huella dactilar. Resistente al polvo.",
                    Price = 240m,
                    Stock = 5,
                    CategoryId = celularesId,
                    ImageUrl = "https://res.cloudinary.com/danl5ulmr/image/upload/v1766193299/celular2_jvjifc.png"
                },
                new Product {
                    Name = "Samsung Galaxy S25 Ultra 512gb Titanium Gray",
                    Description = "Memoria RAM: 12 GB. Dispositivo desbloqueado para que elijas la compañía telefónica que prefieras. Compatible con redes 5G. Pantalla de 6.9. Memoria interna de 512GB.",
                    Price = 1700m,
                    Stock = 0,
                    CategoryId = celularesId,
                    ImageUrl = "https://res.cloudinary.com/danl5ulmr/image/upload/v1766193299/celular3_zjadfg.png"
                },
                new Product {
                    Name = "Apple iPhone 16e (128 Gb) - Blanco",
                    Description = "Memoria RAM: 8 GB. Dispositivo desbloqueado para que elijas la compañía telefónica que prefieras. Pantalla de 6.1. Memoria interna de 128GB. Con reconocimiento facial.",
                    Price = 1024m,
                    Stock = 4,
                    CategoryId = celularesId,
                    ImageUrl = "https://res.cloudinary.com/danl5ulmr/image/upload/v1766193300/celular4_ce4gup.png"
                },
                new Product {
                    Name = "Samsung Galaxy A56 5G Dual Sim 256GB 12GB",
                    Description = "Dispositivo liberado para que elijas la compañía telefónica que prefieras. Compatible con redes 5G. Pantalla Super AMOLED de 6.7. Tiene 3 cámaras traseras de 50Mpx/12Mpx/5Mpx. Cámaras delanteras de 12Mpx. Batería de 5 Ah. Memoria interna de 256GB. A prueba de agua. Con reconocimiento facial y sensor de huella dactilar.",
                    Price = 510m,
                    Stock = 18,
                    CategoryId = celularesId,
                    ImageUrl = "https://res.cloudinary.com/danl5ulmr/image/upload/v1766193300/celular5_lsbvpe.png"
                },
                new Product {
                    Name = "Apple iPhone 13 (128 GB)",
                    Description = "Memoria RAM: 4 GB. Memoria interna: 128 GB. Pantalla Super Retina XDR de 6.1 pulgadas1. Modo Cine con baja profundidad de campo y cambios de enfoque automáticos en tus videos. Sistema avanzado de dos cámaras de 12 MP (gran angular y ultra gran angular) con Estilos Fotográficos, HDR Inteligente 4, modo Noche y grabación de video 4K HDR en Dolby Vision. Cámara frontal TrueDepth de 12 MP con modo Noche y grabación de video 4K HDR en Dolby Vision. Chip A15 Bionic para un rendimiento fuera de serie. Hasta 19 horas de reproducción de video2. Diseño resistente con Ceramic Shield. Resistencia al agua IP68, líder en la industria3. IOS 15 con nuevas funcionalidades para aprovechar tu iPhone al máximo4. Compatibilidad con accesorios MagSafe, que se acoplan fácilmente a tu iPhone y permiten una carga inalámbrica más rápida5.",
                    Price = 605m,
                    Stock = 12,
                    CategoryId = celularesId,
                    ImageUrl = "https://res.cloudinary.com/danl5ulmr/image/upload/v1766193300/celular5_lsbvpe.png"
                },
                new Product {
                    Name = "Apple iPhone 14 (128 GB)",
                    Description = "Pantalla Super Retina XDR de 6.1 pulgadas.(1) Sistema avanzado de cámaras para tomar mejores fotos en cualquier condición de luz. Modo Cine ahora en 4K Dolby Vision de hasta 30cps. Modo Acción para lograr videos estables, aún con cámara en mano. Detección de Choques(2), una funcionalidad de seguridad que pide ayuda cuando tú no puedes. Batería para todo el día y hasta 26 horas de reproducción de vídeo.(3) A15 Bionic, con GPU e 5 núcleos para un rendimiento fuera de serie. Red 5G ultrarrápida.(4) Ceramic Shield y resistencia al agua, características de durabilidad líderes en la industria.(5) IOS 16 ofrece aún más opciones de personalización y más formas de comunicarse y compartir.",
                    Price = 750m,
                    Stock = 5,
                    CategoryId = celularesId,
                    ImageUrl = "https://res.cloudinary.com/danl5ulmr/image/upload/v1766193299/celular7_xxsyzj.png"
                },
                new Product {
                    Name = "Tarjeta Video Gigabyte Amd Radeon Rx 9060 Xt Gaming Oc 16gb",
                    Description = "Tamaño de la memoria: 16 GB. Potencia gráfica total de 160 W para un rendimiento óptimo en juegos. Interfaz PCIe 5.0 para alta velocidad de transferencia. Bus de memoria de 128 bits que mejora el ancho de banda. Resolución máxima de 8K para imágenes ultra nítidas. Compatible con DirectX y OpenGL para un amplio soporte de juegos. Refrigeración por aire que garantiza un funcionamiento eficiente.",
                    Price = 760m,
                    Stock = 21,
                    CategoryId = graficasId,
                    ImageUrl = "https://res.cloudinary.com/danl5ulmr/image/upload/v1766193301/grafica1_bjcm9m.png"
                },
                new Product {
                    Name = "Tarjeta Gráfica MSI GeForce RTX 3060 Ventus 12GB GDDR6 PCI Express 4.0",
                    Description = "Tamaño de la memoria: 12 GB. Interfaz PCI-Express 4.0. Memoria gráfica GDDR6 de 15000MHz. Bus de memoria: 192bit. Cantidad de núcleos: 3584. Frecuencia boost del núcleo de 1807MHz y base de 1320MHz. Resolución máxima: 8K. Compatible con directX y openGL. Requiere de 550W de alimentación. Cuenta con conexión: 3 DisplayPort 1.4. Incluye: Manual de usuario. Ideal para trabajar a alta velocidad. Admite hasta 4 monitores.",
                    Price = 470m,
                    Stock = 5,
                    CategoryId = graficasId,
                    ImageUrl = "https://res.cloudinary.com/danl5ulmr/image/upload/v1766193301/grafica2_oduauv.png"
                },
                new Product {
                    Name = "Placa De Video Palit Geforce Rtx 5060 Ti Dual 8gb Gddr7 Vx",
                    Description = "Tamaño de la memoria: 8 GB. Interfaz PCI-Express 5.0 para máxima compatibilidad y velocidad. Resolución máxima de 8K para imágenes detalladas. Bus de memoria de 128 bits para mayor ancho de banda. Conectividad con 4 salidas para múltiples monitores. Refrigeración por aire para un rendimiento óptimo. Compatible con DirectX y OpenGL para aplicaciones gráficas avanzadas.",
                    Price = 670m,
                    Stock = 8,
                    CategoryId = graficasId,
                    ImageUrl = "https://res.cloudinary.com/danl5ulmr/image/upload/v1766193301/grafica3_fda8ul.png"
                },
                new Product {
                    Name = "Placa De Video Geforce Rtx 5070 12gb Msi Gaming Trio Oc 1",
                    Description = "Tamaño de la memoria: 12 GB. Interfaz PCI Express® Gen 5 para máxima velocidad de transferencia de datos. Bus de memoria de 192 bit para mayor ancho de banda. Compatible con DirectX para aprovechar gráficos avanzados. Cuatro salidas de monitor para configuraciones de múltiples pantallas. Requerimiento energético de 650 W para asegurar un rendimiento óptimo.",
                    Price = 1200m,
                    Stock = 2,
                    CategoryId = graficasId,
                    ImageUrl = "https://res.cloudinary.com/danl5ulmr/image/upload/v1766193301/grafica4_e9qumc.png"
                },
                new Product {
                    Name = "Tarjeta De Video Gigabyte Amd Radeon Rx 9060 Xt Gaming Oc 8g",
                    Description = "Tamaño de la memoria: 8 GB. Interfaz PCI-E 5.0 para una conexión de alta velocidad y eficiencia. Potencia gráfica total de 250 W para un rendimiento óptimo. Soporta resolución máxima de 8K para imágenes ultra nítidas. Incluye tres salidas de monitor para múltiples display. Conectividad HDMI y DisplayPort para versatilidad en el uso. Refrigeración por aire para un funcionamiento estable y eficiente.",
                    Price = 660m,
                    Stock = 7,
                    CategoryId = graficasId,
                    ImageUrl = "https://res.cloudinary.com/danl5ulmr/image/upload/v1766193301/grafica6_dxzivj.png"
                },
            };

            context.Products.AddRange(products);
            await context.SaveChangesAsync();
        }
    }
}
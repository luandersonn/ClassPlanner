using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ClassPlanner.Data;

public class DefaultClassroomDataGenerator(IServiceProvider serviceProvider)
{
    public async Task GenerateDefaultAsync()
    {
        using var scope = serviceProvider.CreateScope();
        AppDbContext dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();

        List<Classroom> classrooms = GetClassrooms();

        await dbContext.Classroom.AddRangeAsync(classrooms);
        await dbContext.SaveChangesAsync();
    }

    private static List<Classroom> GetClassrooms() => [
            new Classroom
            {
                Name = "1ª Série - Tempo Integral",
                Subjects =
                [
                    // Disciplinas básicas
                    new Subject { Name = "Língua Portuguesa", PeriodsPerWeek = 3 },
                    new Subject { Name = "Arte", PeriodsPerWeek = 1 },
                    new Subject { Name = "Língua Inglesa", PeriodsPerWeek = 1 },
                    new Subject { Name = "Educação Física", PeriodsPerWeek = 1 },
                    new Subject { Name = "Matemática", PeriodsPerWeek = 3 },
                    new Subject { Name = "Química", PeriodsPerWeek = 1 },
                    new Subject { Name = "Física", PeriodsPerWeek = 1 },
                    new Subject { Name = "Biologia", PeriodsPerWeek = 2 },
                    new Subject { Name = "Filosofia", PeriodsPerWeek = 1 },
                    new Subject { Name = "Geografia", PeriodsPerWeek = 2 },
                    new Subject { Name = "História", PeriodsPerWeek = 1 },
                    new Subject { Name = "Sociologia", PeriodsPerWeek = 1 },

                    // Itinerários Formativos
                    new Subject { Name = "Formação para Cidadania e Competências Socioemocionais", PeriodsPerWeek = 1 },
                    new Subject { Name = "NTPPS (Núcleo de Trabalho, Pesquisa e Práticas Sociais)", PeriodsPerWeek = 4 },
                    new Subject { Name = "Língua Estrangeira", PeriodsPerWeek = 2 },
                    new Subject { Name = "Estudo Orientado", PeriodsPerWeek = 2 },
                    new Subject { Name = "Aprofundamento em Língua Portuguesa", PeriodsPerWeek = 2 },
                    new Subject { Name = "Aprofundamento em Matemática", PeriodsPerWeek = 2 },
                    new Subject { Name = "Cultura Digital - Letramento Digital", PeriodsPerWeek = 2 },
                    new Subject { Name = "Projeto Integrador", PeriodsPerWeek = 2 },
                    new Subject { Name = "Unidade Curricular Eletiva I", PeriodsPerWeek = 2 },
                    new Subject { Name = "Unidade Curricular Eletiva II", PeriodsPerWeek = 2 },
                    new Subject { Name = "Unidade Curricular Eletiva III", PeriodsPerWeek = 2 },
                    new Subject { Name = "Unidade Curricular Eletiva IV", PeriodsPerWeek = 2 },
                    new Subject { Name = "Clube Estudantil", PeriodsPerWeek = 2 }
                ]
            },
            new Classroom
            {
                Name = "2ª Série - Tempo Integral",
                Subjects =
                [
                    // Disciplinas básicas
                    new Subject { Name = "Língua Portuguesa", PeriodsPerWeek = 3 },
                    new Subject { Name = "Arte", PeriodsPerWeek = 1 },
                    new Subject { Name = "Língua Inglesa", PeriodsPerWeek = 1 },
                    new Subject { Name = "Educação Física", PeriodsPerWeek = 1 },
                    new Subject { Name = "Matemática", PeriodsPerWeek = 3 },
                    new Subject { Name = "Química", PeriodsPerWeek = 2 },
                    new Subject { Name = "Física", PeriodsPerWeek = 1 },
                    new Subject { Name = "Biologia", PeriodsPerWeek = 1 },
                    new Subject { Name = "Filosofia", PeriodsPerWeek = 1 },
                    new Subject { Name = "Geografia", PeriodsPerWeek = 1 },
                    new Subject { Name = "História", PeriodsPerWeek = 2 },
                    new Subject { Name = "Sociologia", PeriodsPerWeek = 1 },

                    // Itinerários Formativos
                    new Subject { Name = "Formação para Cidadania e Competências Socioemocionais", PeriodsPerWeek = 1 },
                    new Subject { Name = "NTPPS (Núcleo de Trabalho, Pesquisa e Práticas Sociais)", PeriodsPerWeek = 4 },
                    new Subject { Name = "Redação", PeriodsPerWeek = 2 },
                    new Subject { Name = "Estudo Orientado", PeriodsPerWeek = 2 },
                    new Subject { Name = "Cultura Digital - Cidadania Digital", PeriodsPerWeek = 2 },
                    new Subject { Name = "Aprofundamento em Língua Portuguesa", PeriodsPerWeek = 2 },
                    new Subject { Name = "Aprofundamento em Matemática", PeriodsPerWeek = 2 },
                    new Subject { Name = "Língua Estrangeira", PeriodsPerWeek = 2 },
                    new Subject { Name = "Educação Física (Itinerário)", PeriodsPerWeek = 2 },
                    new Subject { Name = "Unidade Curricular Eletiva I", PeriodsPerWeek = 2 },
                    new Subject { Name = "Unidade Curricular Eletiva II", PeriodsPerWeek = 2 },
                    new Subject { Name = "Unidade Curricular Eletiva III", PeriodsPerWeek = 2 },
                    new Subject { Name = "Unidade Curricular Eletiva IV", PeriodsPerWeek = 2 },
                ]
            },
            new Classroom
            {
                Name = "3ª Série - Regular",
                Subjects =
                [
                    // Disciplinas básicas
                    new Subject { Name = "Língua Portuguesa", PeriodsPerWeek = 3 },
                    new Subject { Name = "Arte", PeriodsPerWeek = 1 },
                    new Subject { Name = "Língua Inglesa", PeriodsPerWeek = 1 },
                    new Subject { Name = "Educação Física", PeriodsPerWeek = 1 },
                    new Subject { Name = "Matemática", PeriodsPerWeek = 3 },
                    new Subject { Name = "Química", PeriodsPerWeek = 1 },
                    new Subject { Name = "Física", PeriodsPerWeek = 1 },
                    new Subject { Name = "Biologia", PeriodsPerWeek = 2 },
                    new Subject { Name = "Filosofia", PeriodsPerWeek = 1 },
                    new Subject { Name = "Geografia", PeriodsPerWeek = 1 },
                    new Subject { Name = "História", PeriodsPerWeek = 2 },
                    new Subject { Name = "Sociologia", PeriodsPerWeek = 1 },

                    // Itinerários Formativos para o ENEM
                    new Subject { Name = "Formação para Cidadania e Competências Socioemocionais", PeriodsPerWeek = 2 },
                    new Subject { Name = "Linguagens para o ENEM", PeriodsPerWeek = 2 },
                    new Subject { Name = "Matemática para o ENEM", PeriodsPerWeek = 2 },
                    new Subject { Name = "Ciências da Natureza para o ENEM", PeriodsPerWeek = 2 },
                    new Subject { Name = "Ciências Humanas para o ENEM", PeriodsPerWeek = 2 },
                    new Subject { Name = "Redação", PeriodsPerWeek = 2 }
                ]
            }
        ];
}


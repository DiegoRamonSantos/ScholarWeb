# 🎓 ScholarWeb

Uma aplicação web desenvolvida com **ASP.NET Core MVC** para gerenciamento acadêmico e administrativo de uma instituição de ensino. O projeto centraliza cadastros, matrículas, notas, períodos letivos e lançamentos financeiros em um painel simples, organizado e responsivo.

## 📌 Sobre o projeto

O **ScholarWeb** é um sistema acadêmico criado para facilitar a administração de informações escolares em um ambiente web. A aplicação possui autenticação administrativa, dashboard com indicadores importantes e módulos de cadastro para acompanhar alunos, professores, cursos, turmas, disciplinas, matrículas, notas e financeiro.

O projeto foi desenvolvido com foco em organização, usabilidade e manutenção do código, utilizando arquitetura MVC, Entity Framework Core, validações de dados e persistência local com SQLite.

## ✨ Principais funcionalidades

- ✅ Dashboard com indicadores de alunos, professores, cursos e turmas
- ✅ Resumo financeiro com valores recebidos, pendentes e atrasados
- ✅ Cadastro, edição, detalhamento e exclusão/inativação de registros
- ✅ Gestão de alunos com CPF, e-mail, telefone, endereço e status
- ✅ Gestão de professores com formação, especialidade e status
- ✅ Cadastro de cursos, disciplinas, turmas e períodos letivos
- ✅ Matrículas vinculando aluno, curso, turma e período letivo
- ✅ Lançamento de notas com cálculo automático da média final
- ✅ Definição automática da situação do aluno: aprovado, recuperação ou reprovado
- ✅ Controle financeiro por aluno e matrícula
- ✅ Atualização automática de status financeiro: pendente, pago, atrasado ou cancelado
- ✅ Busca nos principais módulos administrativos
- ✅ Validações client-side e server-side
- ✅ Interface responsiva com menu lateral e painel administrativo
- ✅ Persistência local com SQLite

## 🛠️ Tecnologias utilizadas

### Backend
- **ASP.NET Core 10.0**
- **C#**
- **ASP.NET Core MVC**
- **Entity Framework Core 10.0**
- **SQLite**

### Frontend
- **Razor Views**
- **HTML5**
- **CSS personalizado**
- **Bootstrap**
- **JavaScript**
- **Font Awesome**
- **Google Fonts - Inter**

### Ferramentas de desenvolvimento
- **.NET 10 SDK**
- **Entity Framework Core Tools**
- **Migrations do Entity Framework**
- **Visual Studio 2026** ou **Visual Studio Code**

## 🧱 Arquitetura e estrutura do projeto

A aplicação segue uma estrutura MVC organizada:

- **Controllers**: controladores responsáveis pelas rotas e ações dos módulos
- **Controllers/Admin**: controller  para CRUD administrativo reutilizável
- **Models**: entidades do domínio acadêmico e financeiro
- **ViewModels**: modelos específicos para telas, dashboard e componentes administrativos
- **Views**: telas Razor da aplicação
- **Views/Shared/AdminCrud**: telas compartilhadas para listagem, criação, edição, detalhes e exclusão
- **Data**: contexto do Entity Framework, migrations, factory e configuração do banco
- **wwwroot**: arquivos estáticos como CSS, JavaScript, Bootstrap, jQuery e ícones

## 📚 Módulos do sistema

- **Dashboard**: indicadores gerais, resumo financeiro e últimos alunos cadastrados
- **Alunos**: cadastro completo de estudantes
- **Professores**: cadastro de docentes
- **Cursos**: controle de cursos, carga horária e duração
- **Turmas**: organização de turmas por curso, período letivo, turno e capacidade
- **Disciplinas**: vínculo entre curso, professor e carga horária
- **Matrículas**: associação entre aluno, curso, turma e período letivo
- **Notas**: lançamento de notas e cálculo automático de média
- **Financeiro**: controle de mensalidades, matrículas, taxas, materiais e outros lançamentos
- **Períodos letivos**: cadastro e status de períodos acadêmicos

## ⚙️ Requisitos do sistema

Antes de executar o projeto, certifique-se de ter instalado:

- **.NET 10 SDK**
- **Visual Studio 2026** com a carga de trabalho **ASP.NET e desenvolvimento Web** (ou **Visual Studio Code**)
- **Git** (opcional, para clonar o repositório)
- Um navegador web moderno

## 🚀 Como executar localmente

### 1. Clone o repositório

```bash
git clone https://github.com/DiegoRamonSantos/ScholarWeb.git
cd ScholarWeb
```

Ou baixe o projeto como arquivo ZIP e extraia em uma pasta local.

### 2. Abra o projeto no Visual Studio

Instale o Visual Studio 2026 com a carga de trabalho **ASP.NET e desenvolvimento Web** e o **.NET 10 SDK**. Abra o arquivo `ScholarWeb.slnx` na pasta do projeto. O Visual Studio restaurará as dependências NuGet ao carregar ou compilar a solução.

### 3. Execute a aplicação

No Visual Studio, selecione o perfil `https` na barra de execução e pressione **F5**. Para executar sem depuração, pressione **Ctrl+F5**.

A aplicação será aberta no navegador. Com o perfil `https`, os endereços configurados são:

- https://localhost:7224
- http://localhost:5239

### 5. Faça login

1. Acesse a página inicial da aplicação.
2. Informe as credenciais demonstrativas de administrador:
   - **E-mail:** `admin@scholarweb.local`
   - **Senha:** `Admin@123`
3. Após o login, o sistema redirecionará para o dashboard.

> **Atenção:** essas credenciais são públicas e existem exclusivamente para demonstração, testes e estudo. Não utilize essa senha em ambientes de produção. Em um ambiente real, altere a senha e mantenha as credenciais fora do código-fonte.

### 6. Uso básico

1. Cadastre os períodos letivos.
2. Cadastre cursos, professores e disciplinas.
3. Crie turmas vinculadas aos cursos e períodos.
4. Cadastre alunos.
5. Realize matrículas dos alunos nas turmas.
6. Lance notas e acompanhe a situação acadêmica.
7. Registre lançamentos financeiros e acompanhe pagamentos no dashboard.


## 🧠 Conceitos aplicados

- Arquitetura **MVC**
- Separação entre Models, Views, Controllers e ViewModels
- CRUD reutilizável para módulos administrativos
- Entity Framework Core com relacionamentos e migrations
- Validação com Data Annotations
- Injeção de dependências nativa do ASP.NET Core
- Regras de negócio no contexto de dados
- Cálculo automático de média e situação acadêmica
- Atualização automática de status financeiro
- Interface responsiva e componentizada

## 📝 Observações importantes

- O sistema utiliza banco local SQLite e não depende de serviços externos para persistência.
- O acesso é voltado para um usuário administrador.
- O projeto é indicado para estudos, portfólio e evolução para um sistema acadêmico mais completo.
- As credenciais acima são destinadas exclusivamente à demonstração do projeto e não representam credenciais reais ou de produção

## 👨‍💻 Autor

- **Diego Ramon**

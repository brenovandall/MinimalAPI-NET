using AutoMapper;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using MinimalAPI.Infra.Data;
using MinimalAPI.Infra.DTO;
using MinimalAPI.Infra.Helper;
using MinimalAPI.Infra.Models;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddAutoMapper(typeof(MapProfiles));
builder.Services.AddValidatorsFromAssemblyContaining<Program>();
var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}


app.MapGet("/api/get/product", (ILogger<Program> _logger) =>
{
    ApiResponse response = new ApiResponse();

    string loggingTableInformation = null;
    foreach(var item in ProductStorage.productsList)
    {
        loggingTableInformation += item.Name + ",";
    }
    loggingTableInformation = loggingTableInformation.Substring(0, loggingTableInformation.Length - 1);

    _logger.Log(LogLevel.Information, $"Status code: {response.StatusCode} -- Someone consuming info: {loggingTableInformation}");

    if(ProductStorage.productsList is null)
    {
        response.IsSuccess = false;
        response.StatusCode = System.Net.HttpStatusCode.BadRequest;
        response.ErrorMessages.Add("No content avalible");
        _logger.Log(LogLevel.Information, $"Status code: {response.StatusCode.ToString()} -- Someone cannot consume info, error: {response.ErrorMessages[0]}");
        return Results.BadRequest(response);
    }

    response.IsSuccess = true;
    response.Result = ProductStorage.productsList;
    response.StatusCode = System.Net.HttpStatusCode.OK;
    return Results.Ok(response);

}).WithName("GetProducts").Produces<ApiResponse>(200).Produces(400);



app.MapGet("/api/get/product/{id:int}", (int id, ILogger<Program> _logger) => {
    ApiResponse response = new ApiResponse();

    string loggingTableInformation = ProductStorage.productsList.FirstOrDefault(x => x.Id == id).Name;
    _logger.Log(LogLevel.Information, $"Someone consuming: {loggingTableInformation}");

    if (ProductStorage.productsList.FirstOrDefault(x => x.Id == id) is null)
    {
        response.IsSuccess = true;
        response.Result = ProductStorage.productsList.FirstOrDefault(x => x.Id == id);
        response.StatusCode = System.Net.HttpStatusCode.OK;
        _logger.Log(LogLevel.Error, "Something went wrong");
    }

    response.IsSuccess = true;
    response.Result = ProductStorage.productsList.FirstOrDefault(x => x.Id == id);
    response.StatusCode = System.Net.HttpStatusCode.OK;

    return Results.Ok(response);

}).WithName("GetProductById").Produces<ProductDTO>(200).Produces(400);



app.MapPost("/api/product", async ([FromBody] ProductCreateDTO product, ILogger<Program> _logger, IMapper _mapper, IValidator<ProductCreateDTO> _validator) => {

    ApiResponse response = new ApiResponse();

    var validatorResult = await _validator.ValidateAsync(product);

    if (!validatorResult.IsValid)
    {
        var error = validatorResult.Errors.FirstOrDefault().ToString();
        _logger.Log(LogLevel.Error, error);
        return Results.BadRequest(error);
    }

    if (string.IsNullOrEmpty(product.Name)) return Results.BadRequest("Invalid Id");

    var idToCreateTheProduct = ProductStorage.productsList.OrderByDescending(x => x.Id).FirstOrDefault().Id + 1;

    if(ProductStorage.productsList.FirstOrDefault(x => x.Name.ToLower() == product.Name.ToLower()) is not null) return Results.BadRequest("Product already exists");

    var newProduct = _mapper.Map<ProductCreateDTO, ProductDTO>(product);

    newProduct.Id = idToCreateTheProduct;
    newProduct.CreatedAt = DateTime.UtcNow;

    ProductStorage.productsList.Add(newProduct);

    var loggingTableInformation = newProduct;
    _logger.Log(LogLevel.Information, $"Someone created product: {loggingTableInformation.Name} - Id: {loggingTableInformation.Id}");

    if(newProduct is null)
    {
        response.IsSuccess = false;
        response.StatusCode = System.Net.HttpStatusCode.BadRequest;
        response.ErrorMessages.Add(validatorResult.Errors.ToString());
        _logger.Log(LogLevel.Error, response.ErrorMessages.ToString());
        return Results.BadRequest(response);
    }

    response.IsSuccess = true;
    response.Result = ProductStorage.productsList;
    response.StatusCode = System.Net.HttpStatusCode.Created;

    //return Results.Created($"/api/get/product/{product.Id}", product);
    return Results.Ok(response);

}).WithName("CreateProduct").Accepts<ProductCreateDTO>("application/json").Produces<ApiResponse>(201).Produces(400);



app.MapPut("/api/product", (int id, [FromBody] ProductUpdateDTO productToUpdate) => {
    ApiResponse response = new ApiResponse();

    if (string.IsNullOrEmpty(productToUpdate.Name)) return Results.BadRequest();

    var productToUpdateFromStorage = ProductStorage.productsList.FirstOrDefault(x => x.Id == id);

    if(productToUpdateFromStorage is null) return Results.BadRequest();

    productToUpdateFromStorage.Name = productToUpdate.Name;
    productToUpdateFromStorage.Price = productToUpdate.Price;
    productToUpdateFromStorage.IsActive = productToUpdate.IsActive;
    productToUpdateFromStorage.LastUpdate = DateTime.UtcNow;

    response.IsSuccess = true;
    response.Result = productToUpdateFromStorage;
    response.StatusCode = System.Net.HttpStatusCode.OK;

    return Results.Ok(response);

}).WithName("EditProduct");



app.MapDelete("/api/delete/product/{id:int}", (int id) => {
    ApiResponse response = new ApiResponse();

    if (ProductStorage.productsList.FirstOrDefault(x => x.Id == id) is null) return Results.BadRequest();

    var productToDeleteFromStorage = ProductStorage.productsList.FirstOrDefault(x => x.Id == id);

    ProductStorage.productsList.Remove(productToDeleteFromStorage);

    response.IsSuccess = true;
    response.Result = ProductStorage.productsList;
    response.StatusCode = System.Net.HttpStatusCode.OK;

    return Results.Ok(response);
}).WithName("DeleteProduct");

app.UseHttpsRedirection();

app.Run();

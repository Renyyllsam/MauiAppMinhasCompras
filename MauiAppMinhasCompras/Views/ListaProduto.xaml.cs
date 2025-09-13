using MauiAppMinhasCompras.Models;
using System.Collections.ObjectModel;
using Microsoft.Maui.Controls; 

namespace MauiAppMinhasCompras.Views;

public partial class ListaProduto : ContentPage
{
    ObservableCollection<Produto> lista = new ObservableCollection<Produto>();

    public ListaProduto()
    {
        InitializeComponent();

        
        var listView = this.FindByName<ListView>("lst_produtos");
        if (listView != null)
            listView.ItemsSource = lista;
    }

    protected async override void OnAppearing()
    {
        base.OnAppearing();
        try
        {
            lista.Clear();

            List<Produto> tmp = await App.Db.GetAll();

            tmp.ForEach(i => lista.Add(i));
        }
        catch (Exception ex)
        {
            await DisplayAlert("Ops", ex.Message, "OK");
        }
    }

    private void ToolbarItem_Clicked(object sender, EventArgs e)
    {
        try
        {
            Navigation.PushAsync(new NovoProduto());
        }
        catch (Exception ex)
        {
            DisplayAlert("Ops", ex.Message, "OK");
        }
    }

    private async void txt_search_TextChanged(object sender, TextChangedEventArgs e)
    {
        var listView = this.FindByName<ListView>("lst_produtos");
        try
        {
            string q = e.NewTextValue;

            if (listView != null)
                listView.IsRefreshing = true;

            lista.Clear();

            List<Produto> tmp = await App.Db.Search(q);

            tmp.ForEach(i => lista.Add(i));
        }
        catch (Exception ex)
        {
            await DisplayAlert("Ops", ex.Message, "OK");
        }
        finally
        {
            if (listView != null)
                listView.IsRefreshing = false;
        }
    }

    private async void ToolbarItem_Clicked_1(object sender, EventArgs e)
    {
        try
        {
            var produtos = await App.Db.GetAll(); 
            var relatorio = produtos
                .GroupBy(p => p.Categoria)
                .Select(g => new { Categoria = g.Key, Total = g.Sum(p => p.Total) })
                .ToList();

            string msg = string.Join("\n", relatorio.Select(r => $"{r.Categoria}: {r.Total:C}"));
            await DisplayAlert("Relatório de Gastos por Categoria", msg, "OK"); 
        }
        catch (Exception ex)
        {
            await DisplayAlert("Ops", ex.Message, "OK");
        }
    }

    private async void MenuItem_Clicked(object sender, EventArgs e)
    {
        try
        {
            var selecinado = sender as MenuItem;
            if (selecinado?.BindingContext is Produto p)
            {
                bool confirm = await DisplayAlert(
                    "Tem Certeza?", $"Remover {p.Descricao}?", "Sim", "Não");

                if (confirm)
                {
                    await App.Db.Delete(p.Id);
                    lista.Remove(p);
                }
            }
        }
        catch (Exception ex)
        {
            await DisplayAlert("Ops", ex.Message, "OK");
        }
    }

    private void lst_produtos_ItemSelected(object sender, SelectedItemChangedEventArgs e)
    {
        try
        {
            if (e.SelectedItem is Produto p)
            {
                var editar = new EditarProduto
                {
                    BindingContext = p,
                };
                Navigation.PushAsync(editar);
            }
        }
        catch (Exception ex)
        {
            DisplayAlert("Ops", ex.Message, "OK");
        }
    }

    private async void lst_produtos_Refreshing(object sender, EventArgs e)
    {
        var listView = this.FindByName<ListView>("lst_produtos");
        try
        {
            lista.Clear();

            List<Produto> tmp = await App.Db.GetAll();

            tmp.ForEach(i => lista.Add(i));
        }
        catch (Exception ex)
        {
            await DisplayAlert("Ops", ex.Message, "OK");
        }
        finally
        {
            if (listView != null)
                listView.IsRefreshing = false;
        }
    }

    private async void picker_categoria_filtro_SelectedIndexChanged(object sender, EventArgs e)
    {
        var picker = sender as Picker;
        string categoria = picker.SelectedItem?.ToString();

        lista.Clear();
        List<Produto> produtos;
        if (categoria == "Todos")
            produtos = await App.Db.GetAll();
        else
            produtos = (await App.Db.GetAll()).Where(p => p.Categoria == categoria).ToList();

        produtos.ForEach(i => lista.Add(i));
    }
}
# tracker/views.py
from django.shortcuts import render, redirect, get_object_or_404
from django.contrib.auth.forms import AuthenticationForm
from django.contrib.auth import authenticate, login, logout
from django.contrib.auth.decorators import login_required
from django.contrib import messages
from .forms import CustomUserCreationForm, ExpenseForm
from .models import CustomUser, Expense
from datetime import date, timedelta
from django.utils import timezone
from django.http import HttpResponseForbidden
from django.contrib.auth.decorators import login_required
from .forms import ContactForm  # Ensure you import the form


def home(request):
    form = ContactForm()
    success_message = None
    
    if request.method == 'POST':
        form = ContactForm(request.POST)
        if form.is_valid():
            form.save()  # Save form data to the database
            success_message = "Your message has been sent successfully!"
            form = ContactForm()  # Clear the form after successful submission

    return render(request, 'tracker/home.html', {'form': form, 'success_message': success_message})


def about(request):
    return render(request, 'tracker/about.html')  # Ensure you have this template

def contact(request):
    return render(request, 'tracker/contact.html')

def login_view(request):
    if request.method == 'POST':
        form = AuthenticationForm(request, data=request.POST)
        if form.is_valid():
            username = form.cleaned_data.get('username')
            password = form.cleaned_data.get('password')
            user = authenticate(username=username, password=password)
            if user is not None:
                login(request, user)
                return redirect('dashboard')
    else:
        form = AuthenticationForm()
    return render(request, 'tracker/login.html', {'form': form})

def signup_view(request):
    if request.method == 'POST':
        form = CustomUserCreationForm(request.POST)
        if form.is_valid():
            user = form.save()
            login(request, user)
            messages.success(request, 'Registration successful! You are now logged in.')
            return redirect('home')
        else:
            messages.error(request, 'Error during registration. Please check the form.')
    else:
        form = CustomUserCreationForm()
    
    return render(request, 'tracker/signup.html', {'form': form})

def dashboard(request):
    today = timezone.now().date()
    user = request.user
    
    yesterday_expenses = Expense.objects.filter(user=user, date=today-timedelta(days=1))
    today_expenses = Expense.objects.filter(user=user, date=today)
    monthly_expenses = Expense.objects.filter(user=user, date__month=today.month, date__year=today.year)
    yearly_expenses = Expense.objects.filter(user=user, date__year=today.year)
    
    context = {
        'yesterday_expenses': yesterday_expenses,
        'today_expenses': today_expenses,
        'monthly_expenses': monthly_expenses,
        'yearly_expenses': yearly_expenses,
    }
    return render(request, 'tracker/dashboard.html', context)


@login_required
def profile_view(request):
    user = request.user
    return render(request, 'tracker/profile.html', {'user': user})

def logout_view(request):
    logout(request)
    return redirect('home')

def add_expense(request):
    if request.method == 'POST':
        form = ExpenseForm(request.POST)
        if form.is_valid():
            expense = form.save(commit=False)
            expense.user = request.user  # Assuming you have user authentication
            expense.save()
            messages.success(request, 'Expense added successfully!')
            return redirect('dashboard')
        else:
            messages.error(request, 'Error adding expense. Please check the form.')
    else:
        form = ExpenseForm()
    return render(request, 'tracker/add_expense.html', {'form': form})


@login_required
def edit_expense(request, expense_id):
    expense = get_object_or_404(Expense, id=expense_id)
    
    # Authorization check
    if expense.user != request.user:
        return HttpResponseForbidden("You are not allowed to edit this expense.")

    if request.method == 'POST':
        form = ExpenseForm(request.POST, instance=expense)
        if form.is_valid():
            form.save()
            return redirect('dashboard')
    else:
        form = ExpenseForm(instance=expense)
    
    return render(request, 'tracker/edit_expense.html', {'form': form, 'expense': expense})
@login_required
def delete_expense(request, expense_id):
    expense = get_object_or_404(Expense, id=expense_id)
    
    # Authorization check
    if expense.user != request.user:
        return HttpResponseForbidden("You are not allowed to delete this expense.")

    if request.method == 'POST':
        expense.delete()
        return redirect('settings')
    
    return render(request, 'tracker/delete_expense.html', {'expense': expense})

def settings(request):
    expenses = Expense.objects.filter(user=request.user)
    return render(request, 'tracker/settings.html', {'expenses': expenses})

from django.shortcuts import render

def features(request):
    return render(request, 'tracker/features.html')
from .forms import ContactForm




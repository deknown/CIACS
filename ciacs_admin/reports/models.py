from django.db import models


# Create your models here.
class Structure(models.Model):
    id = models.AutoField(primary_key=True)
    parent_id = models.ForeignKey('self', on_delete=models.CASCADE)
    name = models.CharField(max_length=50)


class Computers(models.Model):
    id = models.AutoField
    computer_id = models.IntegerField(null=False)
    ip = models.CharField(max_length=50)

    class Meta:
        unique_together = (('id', 'computer_id'),)


class Os(models.Model):
    name = models.CharField(max_length=50)
    build = models.CharField(max_length=50)
    bits = models.IntegerField(max_length=8)


class Motherboards(models.Model):
    id = models.AutoField(primary_key=True)
    hardware_id = models.ForeignKey(Computers, on_delete=models.CASCADE)
    manufacturer = models.CharField(max_length=250)
    name = models.CharField(max_length=50)

    class Meta:
        unique_together = (('id', 'hardware_id'),)


class Cpus(models.Model):
    id = models.AutoField(primary_key=True)
    hardware_id = models.ForeignKey(Computers, on_delete=models.CASCADE)
    manufacturer = models.CharField(max_length=50)
    name = models.CharField(max_length=50)
    cores = models.IntegerField(max_length=4)
    threads = models.IntegerField(max_length=5)
    speed = models.IntegerField(max_length=5)
    bits = models.IntegerField(max_length=8)

    class Meta:
        unique_together = (('id', 'hardware_id'),)


class Gpus(models.Model):
    id = models.AutoField(primary_key=True)
    hardware_id = models.ForeignKey(Computers, on_delete=models.CASCADE)
    manufacturer = models.CharField(max_length=50)
    name = models.CharField(max_length=50)
    capacity = models.IntegerField(max_length=6)

    class Meta:
        unique_together = (('id', 'hardware_id'),)


class Rams(models.Model):
    id = models.AutoField(primary_key=True)
    hardware_id = models.ForeignKey(Computers, on_delete=models.CASCADE)
    manufacturer = models.CharField(max_length=50)
    name = models.CharField(max_length=50)
    type = models.NullBooleanField
    capacity = models.IntegerField(max_length=15)

    class Meta:
        unique_together = (('id', 'hardware_id'),)


class Disks(models.Model):
    id = models.AutoField(primary_key=True)
    hardware_id = models.ForeignKey(Computers, on_delete=models.CASCADE)
    type = models.CharField(max_length=10)
    capacity = models.IntegerField(max_length=50)

    class Meta:
        unique_together = (('id', 'hardware_id'),)


class Software(models.Model):
    name = models.CharField(max_length=50)
    version = models.CharField(max_length=50)


